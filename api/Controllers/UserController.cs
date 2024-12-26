using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using System.Net;
using System.Net.Http;

namespace api.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IKeycloakTokenService keycloakTokenService;

        public UserController(IKeycloakTokenService keycloakTokenService)
        {
            this.keycloakTokenService = keycloakTokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> AuthorizeAsync([FromBody] KeycloakUserDto keycloakUserDto)
        {
            try
            {
                var response = await keycloakTokenService
                    .GetTokenResponseAsync(keycloakUserDto)
                    .ConfigureAwait(false);

                return new OkObjectResult(response);
            }
            catch (KeycloakException)
            {

                return BadRequest("Authorization has failed!");
            }
            catch (Exception)
            {
                return BadRequest("An error has occured!");
            }
        }

        [Authorize]
        [HttpGet("check/authorization")]
        public IActionResult CheckKeycloakAuthorization()
        {
            return new OkObjectResult(HttpStatusCode.OK);
        }
    }
}
