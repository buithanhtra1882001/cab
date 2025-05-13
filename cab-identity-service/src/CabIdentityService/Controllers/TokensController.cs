using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Controllers.Base;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.Models.Dtos;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Requests;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Controllers
{
    public class TokensController : ApiController<TokensController>
    {
        private readonly ITokenService _tokenService;
        private readonly IFingerprintService _fingerprintService;
        public TokensController(ILogger<TokensController> logger,
            ITokenService tokenService,
            IFingerprintService fingerprintService)
            : base(logger)
        {
            _tokenService = tokenService;
            _fingerprintService = fingerprintService;
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <response code="200">Refresh token successful</response>
        [HttpPost("refresh")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JwtTokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                //if (Request.Cookies.TryGetValue("fingerprint", out string cookieValue))
                //{
                //    cookieValue = cookieValue.Trim();
                //    if (cookieValue == null)
                //        return Unauthorized();
                //}
                //var fingerPrint = cookieValue;
                //var key = refreshTokenRequest.FingerprintHash.Split('-')[1];
                //byte[] actualKey = new byte[key.Length / 2];

                //for (int i = 0; i < actualKey.Length; i++)
                //{
                //    string byteValue = key.Substring(i * 2, 2);
                //    actualKey[i] = Convert.ToByte(byteValue, 16);
                //}

                //var verifyingResult = _fingerprintService.VerifyFingerprintHash(fingerPrint,
                //    refreshTokenRequest.FingerprintHash.Split('-')[0],
                //    actualKey);
                //if (verifyingResult)
                //{
                //    var tokenModel = await _tokenService.RefreshAsync(refreshTokenRequest.RefreshToken);
                //    var httpMessageResponse = new HttpMessageResponse();

                //    if (tokenModel is null)
                //    {
                //        httpMessageResponse.Message = "Refresh token failed";

                //        return BadRequest(httpMessageResponse);
                //    }

                //    return Ok(tokenModel);
                //}
                //else
                //{
                //    return Unauthorized();
                //}

                var tokenModel = await _tokenService.RefreshAsync(refreshTokenRequest.RefreshToken);
                var httpMessageResponse = new HttpMessageResponse();

                if (tokenModel is null)
                {
                    httpMessageResponse.Message = "Refresh token failed";

                    return BadRequest(httpMessageResponse);
                }

                return Ok(tokenModel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Revoke token
        /// </summary>
        /// <response code="200">Revoke token successful</response>
        [HttpPost("revoke")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke(RefreshTokenRequest revokeTokenRequest)
        {
            var result = await _tokenService.RevokeAsync(revokeTokenRequest.RefreshToken);
            var httpMessageResponse = new HttpMessageResponse();

            if (!result)
            {
                httpMessageResponse.Message = "Revoke token failed";

                return BadRequest(httpMessageResponse);
            }

            httpMessageResponse.Message = "Revoke token successful";

            return Ok(httpMessageResponse);
        }
    }
}