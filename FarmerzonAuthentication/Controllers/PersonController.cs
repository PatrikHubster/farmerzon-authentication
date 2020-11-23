using System.Collections.Generic;
using System.Threading.Tasks;
using FarmerzonAuthenticationManager.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthentication.Controllers
{
    [Authorize]
    [Route("person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private IPersonManager PersonManager { get; set; }

        public PersonController(IPersonManager personManager)
        {
            PersonManager = personManager;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(DTO.SuccessResponse<IEnumerable<DTO.PersonOutput>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonAsync([FromQuery] string userName, 
            [FromQuery] string normalizedUserName)
        {
            var people =
                await PersonManager.GetEntitiesAsync(userName: userName, normalizedUserName: normalizedUserName);
            return Ok(new DTO.SuccessResponse<IEnumerable<DTO.PersonOutput>>
            {
                Success = true,
                Content = people
            });
        }
    }
}