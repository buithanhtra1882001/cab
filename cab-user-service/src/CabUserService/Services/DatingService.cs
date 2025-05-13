using AutoMapper;
using CabUserService.Infrastructures.Communications.Http;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.WebSockets;
using System.Security.Policy;

namespace CabUserService.Services
{
    public class DatingService : BaseService<DatingService>, IDatingService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _apiURL;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
        public DatingService(ILogger<DatingService> logger,
            IMapper mapper,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
            : base(logger)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _apiURL = configuration.GetValue<string>("ExternalApis:ApiFPT");
            _apiKey = configuration.GetValue<string>("KeyApis:KeyFPT");

        }

        public async Task<string> CreateOrUpdateDatingProfileAsync(CreateOrUpdateDatingProfileRequest request)
        {

            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user is null)
            {
                return $"Not found user with Id ={request.UserId}";
            }

            var existingUserDatingProfile = await db.UserDatingProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var existingInterests = await db.DatingInterests.Where(x => request.InterestIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
            if (existingUserDatingProfile is null)
            {
                var newUserDatingProfile = _mapper.Map<UserDatingProfile>(request);
                newUserDatingProfile.Id = Guid.NewGuid();
                newUserDatingProfile.InterestIds = string.Join(",", existingInterests);
                newUserDatingProfile.AlbumUrls = string.Join(",", request.AlbumUrls);
                await db.AddAsync(newUserDatingProfile);
            }
            else
            {
                var oldDatingProfile = _mapper.Map(request, existingUserDatingProfile);
                oldDatingProfile.InterestIds = string.Join(";", existingInterests);
                oldDatingProfile.AlbumUrls = string.Join(",", request.AlbumUrls);
                db.UserDatingProfiles.Update(oldDatingProfile);
            }
            try
            {
                await db.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }

        public async Task<PagingResponse<UserDatingProfileResponse>> GetFilteredAndPagedDatingProfilesAsync(GetFilteredAndPagedDatingProfiles request)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

                var query = db.UserDatingProfiles.Where(x =>
                    x.Age >= request.MinAge &&
                    (request.MaxAge == 0 || x.Age <= request.MaxAge) &&
                    (string.IsNullOrEmpty(request.Keyword) || x.Nickname.Contains(request.Keyword)) &&
                    (string.IsNullOrEmpty(request.Gender) || x.Gender == request.Gender) &&
                    (!request.CityId.HasValue || x.CityId == request.CityId) &&
                    (!request.InterestId.HasValue || x.InterestIds.Contains(request.InterestId.ToString()))
                );

                var count = await query.CountAsync();
                var userDatingProfiles = await query.OrderByDescending(x => x.CreatedAt)
                                                    .Skip((request.PageNumber - 1) * request.PageSize)
                                                    .Take(request.PageSize)
                                                    .ToListAsync();

                var result = _mapper.Map<List<UserDatingProfileResponse>>(userDatingProfiles);

                var response = new PagingResponse<UserDatingProfileResponse>()
                {
                    Data = result,
                    Total = count
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving data", ex);
            }
        }

        public async Task<bool> VerifyUserOver18YearsOldFromCccdAsync(Guid auid, IFormFile formFile)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var user = await db.Users.Include(x => x.UserDetail).FirstOrDefaultAsync(x => x.Id == auid);
                if (user is null)
                {
                    var errorMessage = $"Not found user with Id ={auid}";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                var userInformation = await GetUserInformationOnCccdAsync(formFile);
                var userData = userInformation.Data.FirstOrDefault();
                if (userData != null)
                {
                    var dateOfBirthSubstring = userData.Dob.Substring(6, 4);
                    if (int.TryParse(dateOfBirthSubstring, out int birthYear))
                    {
                        var isUserEligibleForDating = DateTime.UtcNow.Year - birthYear >= 18;
                        if (!isUserEligibleForDating)
                        {
                            var errorMessage = $"users with id = {user.Id} under 18 years old";
                            _logger.LogError(errorMessage);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying user's age from CCCD image.");
                throw;
            }
        }
        private async Task<UserInformationOnCccdResponse> GetUserInformationOnCccdAsync(IFormFile formFile)
        {
            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

                        formData.Add(new StreamContent(formFile.OpenReadStream()), "image", formFile.FileName);

                        var response = await httpClient.PostAsync(_apiURL, formData);
                        if (!response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            throw new Exception(result);
                        }
                        return await response.Content.ReadFromJsonAsync<UserInformationOnCccdResponse>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CCCD image recognition response.");
                throw;
            }
        }
    }
}
