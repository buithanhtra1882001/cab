using Autofac.Core;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Infrastructures.Exceptions;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.IntegrationEvents.Events;
using WCABNetwork.Cab.IdentityService.Models.Dtos;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Facebook;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Mail;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Requests;
using WCABNetwork.Cab.IdentityService.Models.Entities;
using WCABNetwork.Cab.IdentityService.Services.Base;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Services.Implements
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private readonly SignInManager<Account> _signInManager;
        private readonly UserManager<Account> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly AppSettings _appSettings;
        private readonly IEventBus _eventBus;
        //private readonly IEventBus _eventBus;
        private readonly IStringLocalizer<AccountService> _localizer;
        private readonly string CONFIRM_EMAIL_API;
        private readonly string NOREPLY_EMAIL_NAME;
        private readonly string NOREPLY_EMAIL_FROM;


        public AccountService(ILogger<AccountService> logger,
            IMapper mapper,
            SignInManager<Account> SignInManager,
            UserManager<Account> userManager,
            ITokenService tokenService,
            IEmailService emailService,
            IEventBus eventBus,
            RoleManager<IdentityRole<int>> roleManager,
            IOptions<AppSettings> options,
            IStringLocalizer<AccountService> localizer
            )
            : base(logger)
        {
            _signInManager = SignInManager;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
            _roleManager = roleManager;
            _appSettings = options.Value;
            _eventBus = eventBus;
            _localizer = localizer;
            CONFIRM_EMAIL_API = "/account/verify-email";
            NOREPLY_EMAIL_NAME = "CAB";
            NOREPLY_EMAIL_FROM = "cabplatformsvn@gmail.com";
        }

        public bool CheckByEmail(string email)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.NormalizedEmail.Equals(email.ToUpper()));

            return user is not null;
        }

        public async Task ResendConfirmationEmailAsync(string email)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.NormalizedEmail.Equals(email.ToUpper()));

            if (user is null)
            {
                _logger.LogInformation($"User is not exists, email: {email}.");

                throw new BusinessException($"User is not exists.");
            }

            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            if (isEmailConfirmed)
            {
                throw new BusinessException($"{email} is already confirmed.");
            }

            await SendConfirmationEmailAsync(user);
        }

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Uuid == userId);

            if (user is null)
            {
                _logger.LogInformation($"User is not exists, userId: {userId}.");

                throw new AppException($"User is not exists.");
            }

            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            if (isEmailConfirmed)
            {
                throw new AppException($"Email for user {userId} is already confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Email for user {userId} confirmed.");
                 _eventBus.Publish(new UserRegisterIntegrationEvent
                    (user.Uuid, "", "", "", true));
                return;
            }

            _logger.LogInformation($"Confirm failed for user {userId}, reason: {result.ToString()}");

            throw new AppException($"Confirm failed.");
        }

        public async Task<string> RegisterAsync(RegisterRequest registerRequest)
        {
            var account = _mapper.Map<Account>(registerRequest);

            var registerAccountResult = await _userManager.CreateAsync(
                account,
                registerRequest.Password
            );

            if (registerAccountResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(account, "User");

                await SendConfirmationEmailAsync(account);

                _logger.LogInformation($"Sent confirmation link to {registerRequest.Email}.");

                _eventBus.Publish(new UserRegisterIntegrationEvent
                    (account.Uuid, registerRequest.Email, registerRequest.FullName, registerRequest.UserName, false));
                return account.Uuid;
            }

            _logger.LogWarning(
                "Can not create account, errors: {@Error}",
                registerAccountResult.Errors
            );


            var message = registerAccountResult.Errors.FirstOrDefault()?.Description ?? string.Empty;

            throw new BusinessException(message);
        }

        public async Task<LoginReturnType> LoginAsync(LoginRequest loginRequest)
        {
            var loginReturnType = new LoginReturnType();
            var result = await _signInManager.PasswordSignInAsync(
                loginRequest.Email,
                loginRequest.Password,
                false,
                true);

            if (result.Succeeded)
            {
                var account = await _userManager.FindByEmailAsync(loginRequest.Email);
                var tokenModel = await _tokenService.GenerateAsync(account);

                if (!await _userManager.IsEmailConfirmedAsync(account))
                {
                    var message = $"Email {account.Email} has not been confirmed, please confirm email";
                    throw new AppException(CabIdentityService.Constants.AppError.EMAIL_NOT_VERIFIED, message);
                }

                loginReturnType.token = tokenModel;
            }
            else if (result.IsLockedOut)
            {
                loginReturnType.accountLockedOut = true;
            }
            else loginReturnType.incorrectCredential = true;

            return loginReturnType;
        }

        public async Task RequestPasswordResetAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var requestUser = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);

            if (requestUser == null)
            {
                _logger.LogWarning($"Cannot reset password for this account, errors: Account with {resetPasswordRequest.Email} not found!");
                var message = "Account not found!";
                throw new BusinessException(message);
            }
            else
            {
                if (requestUser.IsSoftDeleted)
                {
                    _logger.LogWarning("Cannot reset password for this account, errors: account deactived");
                    var message = "Account not found!";
                    throw new BusinessException(message);
                }
            }

            // Generate a unique token for password reset
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(requestUser);

            await _emailService.SendAsync(new SendEmailConfig(
                NOREPLY_EMAIL_NAME,
                NOREPLY_EMAIL_FROM,
                "Ho va ten",
                resetPasswordRequest.Email,
                _localizer["reset-password-email-subject"],
                _localizer["reset-password-email-template", $"https://cab.vn/account/resetpassword/{resetToken}"]
            ));
        }

        public async Task SetNewPasswordAsync(SetNewPasswordRequest setNewPasswordRequest)
        {
            var requestUser = await _userManager.FindByEmailAsync(setNewPasswordRequest.Email);

            if (requestUser == null)
            {
                _logger.LogWarning($"Cannot set password for this account, errors: Account with {setNewPasswordRequest.Email} not found!");
                var message = "Account not found!";
                throw new BusinessException(message);
            }
            else
            {
                if (requestUser.IsSoftDeleted)
                {
                    _logger.LogWarning("Cannot set password for this account, errors: account deactived");
                    var message = "Account not found!";
                    throw new BusinessException(message);
                }
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(requestUser, setNewPasswordRequest.Token, setNewPasswordRequest.Password);

            if (resetPassResult.Succeeded)
            {
                _logger.LogInformation($"Set password {setNewPasswordRequest.Email} successful!");

                return;
            }

            _logger.LogInformation($"Set password failed, reason: {resetPassResult.ToString()}");

            throw new BusinessException($"Set password failed.");
        }

        public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            var account = await _userManager.Users.FirstOrDefaultAsync( x => x.Uuid == changePasswordRequest.UserId.ToString());

            if (account.IsSoftDeleted)
            {
                _logger.LogWarning("Change password failed. This user is deleted.");
                throw new BusinessException($"Change password failed. This user is deleted.. UserId: {account.Uuid}");
            }

            var result = await _userManager.ChangePasswordAsync(
                account,
                changePasswordRequest.CurrentPassword,
                changePasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Cannot change the password for this account, errors: {@Error}", result.Errors);
                var message = result.Errors.FirstOrDefault()?.Description ?? string.Empty;
                throw new BusinessException(message);
            }
        }

        public async Task<JwtTokenModel> FacebookLoginAsync(ExternalLoginRequest externalLoginRequest)
        {
            var facebookProvider = _appSettings.Providers.FirstOrDefault(x => x.Name.Equals("Facebook"));
            if (facebookProvider is null)
                return null;

            // Get user
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v11.0/")
            };
            var loginResponse = await httpClient.GetAsync(
                $"me?access_token={externalLoginRequest.AccessToken}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture"
            );
            var userResult = await loginResponse.Content.ReadAsStringAsync();
            var userFacebookInfo = JsonConvert.DeserializeObject<FacebookUserData>(userResult);

            if (userFacebookInfo is null)
                return null;

            var userLoginInfo = new UserLoginInfo(facebookProvider.Name, userFacebookInfo.Id, facebookProvider.Name);

            // Find user Login with Facebook in system
            var loginUser = await _userManager.FindByLoginAsync(facebookProvider.Name, userLoginInfo.ProviderKey);
            if (loginUser is null)
            {
                loginUser = await _userManager.FindByEmailAsync(userFacebookInfo.Email);
                if (loginUser is null)
                {
                    loginUser = new Account { Email = userFacebookInfo.Email, UserName = userFacebookInfo.Email };
                    await _userManager.CreateAsync(loginUser);
                    _eventBus.Publish(new UserRegisterIntegrationEvent(loginUser.Uuid, loginUser.Email, loginUser.UserName, loginUser.UserName, true));
                    //prepare and send an email for the email confirmation
                    await _userManager.AddToRoleAsync(loginUser, "User");
                    await _userManager.AddLoginAsync(loginUser, userLoginInfo);
                }
                else
                {
                    if (loginUser.IsSoftDeleted)
                        return null;

                    await _userManager.AddLoginAsync(loginUser, userLoginInfo);
                }
            }

            var tokenModel = await _tokenService.GenerateAsync(loginUser);

            return tokenModel;
        }

        public async Task<JwtTokenModel> GoogleLoginAsync(ExternalLoginRequest externalLoginRequest)
        {
            var googleProvider = _appSettings.Providers.Where(x => x.Name.Equals("Google")).FirstOrDefault();
            if (googleProvider is null)
                return null;

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { googleProvider.AppId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(externalLoginRequest.AccessToken, settings);

            var info = new UserLoginInfo(googleProvider.Name, payload.Subject, googleProvider.Name);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user is null)
                {
                    user = new Account { Email = payload.Email, UserName = payload.Email };
                    await _userManager.CreateAsync(user);
                    _eventBus.Publish(new UserRegisterIntegrationEvent(user.Uuid, user.Email, user.UserName, user.UserName, true));
                    // prepare and send an email for the email confirmation
                    await _userManager.AddToRoleAsync(user, "User");
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    if (user.IsSoftDeleted)
                        return null;

                    await _userManager.AddLoginAsync(user, info);
                }
            }

            var tokenModel = await _tokenService.GenerateAsync(user);

            return tokenModel;
        }
        private async Task SendConfirmationEmailAsync(Account account)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(account);
            var encodedUserId = WebUtility.UrlEncode(account.Uuid);
            var encodedToken = WebUtility.UrlEncode(token);

            await _emailService.SendAsync(new SendEmailConfig(
                    NOREPLY_EMAIL_NAME,
                    NOREPLY_EMAIL_FROM,
                    // TODO: Still send email address instead username
                    account.UserName,
                    account.Email,
                    "Xác minh tài khoản",
                    $"<p>Hãy click vào <a href=\"{_appSettings.FrontendHostURL}{CONFIRM_EMAIL_API}?userId={encodedUserId}&token={encodedToken}\">để xác thực.</p><p>Nếu <strong>không phải bạn thực hiện điều này</strong>" +
                           $", tuyệt đối không click vào đường dẫn hoặc cung cấp đường dẫn cho bất cứ ai.</p><p>Trân trọng.</p><strong>CAB.VN</strong>"
                ));
        }

    }
}
