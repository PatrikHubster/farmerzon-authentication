using System.Threading.Tasks;
using AuthenticationManager.Interface;
using AuthenticationErrorHandling.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DAO = AuthenticationDataAccessModel;
using DTO = AuthenticationDataTransferModel;

namespace Authentication.Controllers
{
    [Route("authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase 
    {
        private IAuthManager AuthManager { get; set; }

        public AuthenticationController(IAuthManager authManager)
        {
            AuthManager = authManager;
        }
        
        /// <summary>
        /// Register a new person on this plattform.
        /// </summary>
        /// <param name="registrationRequest">Includes all necessary data to register a new liter.</param>
        /// <returns>
        /// A bad request if the data aren't valid, an ok message if everything was fine or an internal server error if
        /// something went wrong.
        /// </returns>
        /// <response code="200">Person was created successfully</response>
        /// <response code="400">Person data wasn't valid.</response>
        /// <response code="500">Something unexpected happened.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] DTO.Registration registrationRequest)
        {
            var token = await AuthManager.RegisterAccountAsync(registrationRequest);
            return Ok(new AuthenticationResponse
            {
                Success = true,
                Token = token
            });
        }  
        
        /// <summary>
        /// This endpoint is for logging in a person.
        /// </summary>
        /// <param name="loginByUserNameRequest">Includes all necessary data to login a new liter.</param>
        /// <returns>
        /// Unauthorized if the data aren't valid, an ok message if username and password are correct or a internal
        /// server error if something went wrong.
        /// </returns>
        /// <response code="200">Person was logged in successfully</response>
        /// <response code="400">Person data wasn't valid.</response>
        /// <response code="500">Something unexpected happened.</response>
        [HttpPost("login-username")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUserName([FromBody] DTO.LoginByUserName loginByUserNameRequest)
        {
            var token = await AuthManager.LoginAccountAsync(loginByUserNameRequest);
            return Ok(new AuthenticationResponse
            {
                Success = true,
                Token = token
            });
        }
    }
}