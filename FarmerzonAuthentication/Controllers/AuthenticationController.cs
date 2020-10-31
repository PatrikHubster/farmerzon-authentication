using System;
using System.Threading.Tasks;
using FarmerzonAuthenticationManager.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthentication.Controllers
{
    [Route("authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationManager AuthManager { get; set; }

        public AuthenticationController(IAuthenticationManager authManager)
        {
            AuthManager = authManager;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(DTO.SuccessResponse<DTO.TokenOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody] DTO.RegistrationInput registration)
        {
            var token = await AuthManager.RegisterUserAsync(registration);
            return Ok(new DTO.SuccessResponse<DTO.TokenOutput>()
            {
                Success = true,
                Content = token
            });
        }

        [HttpPost("login-username")]
        [ProducesResponseType(typeof(DTO.SuccessResponse<DTO.TokenOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUserNameAsync([FromBody] DTO.UserNameLoginInput userNameLogin)
        {
            throw new NullReferenceException("This is a message.");
            var token = await AuthManager.LoginUserAsync(userNameLogin);
            return Ok(new DTO.SuccessResponse<DTO.TokenOutput>()
            {
                Success = true,
                Content = token
            });
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(DTO.SuccessResponse<DTO.TokenOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] DTO.RefreshTokenInput refreshToken)
        {
            var token = await AuthManager.RefreshTokenAsync(refreshToken);
            return Ok(new DTO.SuccessResponse<DTO.TokenOutput>()
            {
                Success = true,
                Content = token
            });
        }
    }
}