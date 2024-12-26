using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Exceptions;
using Shared.Settings;
using Shared.Utils;
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

        /*[HttpPost("token")]
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
        } */

        [HttpGet("login")]
        public IActionResult Login([FromServices] IOptions<KeycloakSettings> keycloakOptions)
        {
            var keycloakSettings = keycloakOptions.Value;

            // Generate a random state
            var randomState = OAuthUtils.GenerateRandomState();

            // Store the state securely in the session
            HttpContext.Session.SetString("OAuthState", randomState);

            var authorizationUrl = $"{keycloakSettings.BaseUrl}/protocol/openid-connect/auth" +
                                   $"?response_type=code" +
                                   $"&client_id={keycloakSettings.ClientId}" +
                                   $"&redirect_uri={keycloakSettings.RedirectUri}" +
                                   $"&scope={keycloakSettings.Scope}" +
                                   $"&state={randomState}"; // Optionally use a unique state value to prevent CSRF attacks

            return Redirect(authorizationUrl);
        }


        [HttpPost("token")]
        public async Task<IActionResult> AuthorizeAsync([FromBody] KeycloakAuthCodeDto authCodeDto)
        {
            if (authCodeDto == null || string.IsNullOrEmpty(authCodeDto.AuthorizationCode) || string.IsNullOrEmpty(authCodeDto.RedirectUri))
            {
                return BadRequest("Invalid authorization code or redirect URI");
            }

            try
            {
                var response = await keycloakTokenService
                    .GetTokenResponseAsync(authCodeDto)
                    .ConfigureAwait(false);

                if (response == null)
                {
                    return BadRequest("Failed to retrieve token from Keycloak");
                }

                return Ok(response);
            }
            catch (KeycloakException ex)
            {
                return BadRequest(new
                {
                    Message = "Authorization has failed!",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "An error has occurred!",
                    Details = ex.Message
                });
            }
        }

        // This is the callback endpoint that Keycloak will redirect to after the user logs in
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code , string state)
        {
            // Retrieve the stored state from the session
            var storedState = HttpContext.Session.GetString("OAuthState");

            // Construct the redirect URI dynamically based on the current request
            var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/user/callback";

            // Validate the state parameter
            if (string.IsNullOrEmpty(state) || state != storedState)
            {
                return BadRequest("Invalid state parameter");
            }

            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code is missing");

            var authCodeDto = new KeycloakAuthCodeDto
            {
                AuthorizationCode = code,
                RedirectUri = "http://localhost:5198/api/user/callback"  // Replace with your actual redirect URI
            };

            try
            {
                var tokenResponse = await keycloakTokenService.GetTokenResponseAsync(authCodeDto);

                // You can return the token or a success message as needed
                return Ok(tokenResponse);  // Return the access token or a success message here
            }
            catch (KeycloakException ex)
            {
                return BadRequest(new
                {
                    Message = "Failed to exchange authorization code for tokens.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "An error occurred while processing the authorization code.",
                    Details = ex.Message
                });
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
