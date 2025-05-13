using AutoMapper;
using CabUserService.Infrastructures.Repositories;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;

namespace CabUserService.Services
{
    public class UserCategoryService : BaseService<UserCategoryService>, IUserCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        public UserCategoryService(ILogger<UserCategoryService> logger, IMapper mapper, IServiceProvider serviceProvider) : base(logger)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> FollowCategoriesAsync(Guid userId, List<Guid> categoryIds)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
                var userCategoryRepository = _serviceProvider.GetRequiredService<IUserCategoryRepository>();
                var user = await userRepository.GetByIdAsync(userId);
                if (user is null || user.IsSoftDeleted)
                {
                    return "User not found";
                }
                var categoryExcute = new List<UserCategory>();
                var categoriesNotFound = new List<string>();
                var categoriesAlreadyFollowed = new List<string>();
                foreach (var categoryId in categoryIds)
                {
                    var category = await categoryRepository.GetByIdAsync(categoryId);
                    if (category is null)
                    {
                        categoriesNotFound.Add(categoryId.ToString());
                        continue;
                    }
                    categoriesAlreadyFollowed.Add(category.Name);
                    var existingUserCategory = await userCategoryRepository.GetUserCategoryByIdAsync(userId, categoryId);
                    if (existingUserCategory == null)
                    {
                        var newUserCategory = new UserCategory
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            CategoryId = categoryId
                        };
                        categoryExcute.Add(newUserCategory);
                    }
                }
                if (!categoryExcute.Any())
                {
                    return $"Categories already followed is : {string.Join(", ", categoriesAlreadyFollowed)}, Categories id is not found is : {string.Join(", ", categoriesNotFound)}";

                }
                await userCategoryRepository.AddRangeUserCategoryAsync(categoryExcute);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
        public async Task<List<CategoryResponse>> GetUserFollowedCategoriesAsync(Guid userId)
        {
            try
            {
                var userCategoryRepository = _serviceProvider.GetRequiredService<IUserCategoryRepository>();
                var followedCategories = await userCategoryRepository.GetUserFollowedCategoriesAsync(userId);
                return _mapper.Map<List<CategoryResponse>>(followedCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFollowedCategoriesAsync for user {UserId}", userId);
                return new List<CategoryResponse>();
            }

        }
    }
}
