using CabIdentityService.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Controllers.Base;
using WCABNetwork.Cab.IdentityService.Infrastructures.Exceptions;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.Models.Dtos;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Requests;
using WCABNetwork.Cab.IdentityService.Models.Dtos.User;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Controllers
{
    public class AccountsController : ApiController<AccountsController>
    {
        private readonly IAccountService _accountService;
        private readonly IFingerprintService _fingerPrintService;
        public AccountsController(ILogger<AccountsController> logger,
            IAccountService accountService,
            IFingerprintService fingerPrintService)
            : base(logger)
        {
            _accountService = accountService;
            _fingerPrintService = fingerPrintService;
        }

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <response code="200">Confirm success.</response>
        /// <response code="400">Confirm failed.</response>
        [HttpGet("email/confirm")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmUserEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                // Silently return 400 without message
                _logger.LogInformation("Empty userId or token.");
                return BadRequest(new HttpMessageResponse("UserId and token are required."));
            }

            try
            {
                await _accountService.ConfirmEmailAsync(userId, token);
                _logger.LogInformation($"Email for user {userId} confirmed.");
                return Ok(new HttpMessageResponse("Email confirmation successful."));
            }
            catch (BusinessException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new HttpMessageResponse(e.Message));
            }
        }

        /// <summary>
        /// Test Endpoint
        /// </summary>
        /// <response code="200">Confirm success.</response>
        /// <response code="400">Confirm failed.</response>
        [HttpGet("test")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Test()
        {
            return Ok("hello world");
        }

        /// <summary>
        /// Check if exist an account with provided email address
        /// </summary>
        /// <response code="200">Exist or not exist.</response>
        [HttpGet("check")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CheckUserByEmailResponse), StatusCodes.Status200OK)]
        public IActionResult Check(string email)
        {
            var response = new CheckUserByEmailResponse();
            response.IsExist = _accountService.CheckByEmail(email);

            return Ok(new
            {
                Data = response
            });
        }

        /// <summary>
        /// Register an user account
        /// </summary>
        /// <response code="200">Register success.</response>
        /// <response code="400">Register failed.</response>
        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var userId = await _accountService.RegisterAsync(registerRequest);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new HttpMessageResponse("Register failed."));
            }

            return Ok(new HttpMessageResponse("Register success."));
        }

        [HttpGet("resend-email-confirmation")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            try
            {
                await _accountService.ResendConfirmationEmailAsync(email);
                return Ok(new HttpMessageResponse { Message = "Confirmation email sent successfully." });
            }
            catch (BusinessException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new HttpMessageResponse(e.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Perform login and return JWT Tokens
        /// </summary>
        /// <response code="200">Login successful</response>
        /// <response code="401">Authentication is failed</response>  
        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JwtTokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!CheckPassCode(loginRequest.PassCode))
                return Unauthorized(new HttpMessageResponse("PassCode is invalid"));

            var tokenModel = await _accountService.LoginAsync(loginRequest);
            if (tokenModel.incorrectCredential)
                return Unauthorized(new HttpMessageResponse("Email or Password incorrect"));

            if (tokenModel.accountLockedOut)
            {
                return Unauthorized(
                    new HttpMessageResponse(
                        "Account locked out after multiple failed login attempts. Try again after a few minutes"
                    )
                );
            }

            //Calculate fgpr
            //string fingerprint = _fingerPrintService.GenerateFingerprint(loginRequest.Email.Length + loginRequest.Password.Length);

            //var fingerprintHash = _fingerPrintService.CreateFingerprintHash(fingerprint);
            //tokenModel.token.FingerprintHash = fingerprintHash;
            //var cookieOptions = new CookieOptions
            //{
            //    Secure = false,
            //    HttpOnly = true,
            //    Expires = System.DateTimeOffset.Now.AddDays(7)
            //};

            //Response.Cookies.Append("fingerprint", fingerprint, cookieOptions);

            return Ok(tokenModel.token);
        }

        /// <summary>
        /// Request reset password
        /// </summary>
        /// <response code="200">Send reset password request successful</response>
        [HttpPost("password-request")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            try
            {
                await _accountService.RequestPasswordResetAsync(resetPasswordRequest);
                _logger.LogInformation($"Sent reset password to {resetPasswordRequest.Email}.");

                return Ok(new HttpMessageResponse("Request reset password successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"RequestResetPassword error {ex.Message}");
                return BadRequest(new HttpMessageResponse("Request reset password failed"));
            }
        }

        /// <summary>
        /// Set new password from reset request
        /// </summary>
        /// <response code="200">Change password successful</response>
        [HttpPost("password")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetNewPassword(SetNewPasswordRequest setNewPasswordRequest)
        {
            try
            {
                await _accountService.SetNewPasswordAsync(setNewPasswordRequest);

                return Ok(new HttpMessageResponse("Set new password successful"));
            }
            catch (BusinessException e)
            {
                _logger.LogError(e.Message);

                return BadRequest(new HttpMessageResponse("Set new password failed."));
            }
        }

        /// <summary>
        /// Change the old to new password
        /// </summary>
        /// <response code="200">Change password successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPut("password")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(Guid auid, ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                changePasswordRequest.UserId = auid;
                await _accountService.ChangePasswordAsync(changePasswordRequest);

                return Ok(new HttpMessageResponse("Change password successful"));
            }
            catch (BusinessException e)
            {
                _logger.LogError(e.Message);

                return BadRequest();
            }
        }

        /// <summary>
        /// Login with 3rd platform
        /// </summary>
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin(ExternalLoginRequest externalLoginRequest)
        {
            if (!CheckPassCode(externalLoginRequest.PassCode))
                return Unauthorized(new HttpMessageResponse("PassCode is invalid"));

            _ = new JwtTokenModel();
            JwtTokenModel tokenModel;
            if (externalLoginRequest.Provider.Equals("Facebook"))
                tokenModel = await _accountService.FacebookLoginAsync(externalLoginRequest);
            else
                tokenModel = await _accountService.GoogleLoginAsync(externalLoginRequest);

            if (tokenModel is null)
                return Unauthorized(new HttpMessageResponse("External login failed."));

            return Ok(tokenModel);
        }

        private static bool CheckPassCode(string passcode)
        {
            return passcode == CommonConstant.PassCode;
        }
    }
}
