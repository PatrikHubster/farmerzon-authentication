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
        
        /// <summary>
        /// Register a new person on this platform.
        /// </summary>
        /// <param name="registration">Includes all necessary data to register a new liter.</param>
        /// <returns>
        /// A bad request if the data aren't valid, an ok message if everything was fine or an internal server error if
        /// something went wrong.
        /// </returns>
        /// <response code="200">Person was created successfully</response>
        /// <response code="400">Person data wasn't valid.</response>
        /// <response code="500">Something unexpected happened.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(DTO.SuccessResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody] DTO.RegistrationInput registration)
        {
            var token = await AuthManager.RegisterAccountAsync(registration);
            return Ok(new DTO.SuccessResponse<string>()
            {
                Success = true,
                Content = token
            });
        }  
        
        /// <summary>
        /// This endpoint is for logging in a person.
        /// </summary>
        /// <param name="userNameLogin">Includes all necessary data to login a new liter.</param>
        /// <returns>
        /// Unauthorized if the data aren't valid, an ok message if username and password are correct or a internal
        /// server error if something went wrong.
        /// </returns>
        /// <response code="200">Person was logged in successfully</response>
        /// <response code="400">Person data wasn't valid.</response>
        /// <response code="500">Something unexpected happened.</response>
        [HttpPost("login-username")]
        [ProducesResponseType(typeof(DTO.SuccessResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUserNameAsync([FromBody] DTO.UserNameLoginInput userNameLogin)
        {
            var token = await AuthManager.LoginAccountAsync(userNameLogin);
            return Ok(new DTO.SuccessResponse<string>()
            {
                Success = true,
                Content = token
            });
        }
    }
}