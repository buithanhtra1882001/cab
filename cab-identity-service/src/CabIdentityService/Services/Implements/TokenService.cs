using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.DomainCommands.Commands;
using WCABNetwork.Cab.IdentityService.Infrastructures.Extensions;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.Models.Entities;
using WCABNetwork.Cab.IdentityService.Services.Base;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Services.Implements
{
    public class TokenService : BaseService<TokenService>, ITokenService
    {
        private readonly IUserRefreshTokenRepository _userRefreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly AppSettings _appSettings;
        private readonly UserManager<Account> _userManager;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(ILogger<TokenService> logger,
            IUserRefreshTokenRepository userRefreshTokenRepository,
            UserManager<Account> userManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IOptions<AppSettings> options,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor)
            : base(logger)
        {
            _userManager = userManager;
            _userRefreshTokenRepository = userRefreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _appSettings = options.Value;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<JwtTokenModel> GenerateAsync(Account account)
        {
            var randomString = Guid.NewGuid().ToString();

            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                "fingerprint",
                randomString,
                new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                }
            );

            var userRoles = await _userManager.GetRolesAsync(account);
            var claims = new Dictionary<string, object>
            {
                {"Uuid", account.Uuid },
                {"Role", userRoles },
                {"Fingerprint_Hash", randomString.Sha256() }
            };
            var tokenModel = _jwtTokenGenerator.Generate(claims);
            tokenModel.FingerprintHash = randomString.Sha256();
            var refreshToken = GenerateUniqueToken();
            var expiration = DateTime.UtcNow.AddSeconds(_appSettings.Authentication.RefreshTokenExpiredTimeInSecond);
            var userRefreshToken = new UserRefreshToken()
            {
                TokenValue = refreshToken,
                Expiration = expiration,
                IsRevoked = false,
                SubjectId = account.Id
            };
            tokenModel.RefreshToken = refreshToken;

            await _userRefreshTokenRepository.CreateAsync(userRefreshToken);

            var userRefreshTokenCreatedCommand = new UserRefreshTokenCreatedCommand
            {
                Account = account
            };
            await _mediator.Publish(userRefreshTokenCreatedCommand);

            return tokenModel;
        }

        public async Task<JwtTokenModel> RefreshAsync(string token)
        {
            var userRefreshToken = await _userRefreshTokenRepository.GetByTokenValueAsync(token);

            if (userRefreshToken is null || userRefreshToken.IsRevoked)
                return null;

            var account = await _userManager.FindByIdAsync(userRefreshToken.SubjectId.ToString());

            if (account is null || account.IsSoftDeleted)
                return null;

            var tokenModel = await GenerateAsync(account);

            await RevokeAsync(token);
            return tokenModel;
        }

        public async Task<bool> RevokeAsync(string token)
        {
            var userRefreshToken = await _userRefreshTokenRepository.GetByTokenValueAsync(token);

            if (userRefreshToken is null || userRefreshToken.IsRevoked)
                return false;

            userRefreshToken.IsRevoked = true;

            return await _userRefreshTokenRepository.UpdateAsync(userRefreshToken);
        }

        private string GenerateUniqueToken() => GenerateUuidAsString() + GenerateUuidAsString();

        private string GenerateUuidAsString()
        {
            var trimString = "==";
            var base64String = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return base64String.Substring(0, base64String.Length - trimString.Length);
        }
    }
}