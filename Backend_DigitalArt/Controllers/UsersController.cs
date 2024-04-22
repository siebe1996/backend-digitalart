using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Users;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //The refresh token is being sent in an HTTP only cookie and header
        private void SetTokenCookie(string token)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(2), //TOKEN REFRESH
                IsEssential = true,
            };

            Response.Cookies.Append("Backend_DigitalArt.RefreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }

        [EnableCors("AllowAnyOrigins")]
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ActionResult<PostAuthenticateResponseModel>> Authenticate(PostAuthenticateRequestModel postAuthenticateRequestModel)
        {
            try
            {
                PostAuthenticateResponseModel postAuthenticateResponseModel = await _userRepository.Authenticate(postAuthenticateRequestModel, IpAddress());
                SetTokenCookie(postAuthenticateResponseModel.RefreshToken);
                return Ok(postAuthenticateResponseModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Renew-token")]
        public async Task<ActionResult<PostAuthenticateResponseModel>> RenewToken()
        {
            try
            {
                string refreshToken = Request.Cookies["Backend_DigitalArt.RefreshToken"];
                PostAuthenticateResponseModel postAuthenticateResponseModel = await _userRepository.RenewToken(refreshToken, IpAddress());
                SetTokenCookie(postAuthenticateResponseModel.RefreshToken);
                return Ok(postAuthenticateResponseModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Deactivate-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateToken(PostDeactivateTokenRequestModel postDeactivateTokenRequestModel)
        {
            try
            {
                // Accept token from request body or cookie
                string token = postDeactivateTokenRequestModel.Token ?? Request.Cookies["Backend_DigitalArt.RefreshToken"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("refresh token is required");
                }
                await _userRepository.DeactivateToken(token, IpAddress());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                // Validate and process the reset password request
                bool passwordResetSuccessful = await _userRepository.ResetPassword(model);

                if (passwordResetSuccessful)
                {
                    return Ok(new { Message = "Password reset successful." });
                }
                else
                {
                    return BadRequest(new { Message = "Password reset failed." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal Server Error: {ex.Message}" });
            }
        }*/

        [HttpGet]
        public async Task<ActionResult<GetUserModel>> GetUsers()
        {
            var models = await _userRepository.GetUsers();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("artists")]
        public async Task<ActionResult<GetUserModel>> GetArtists()
        {
            var models = await _userRepository.GetArtists();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("me")]
        public async Task<ActionResult<GetUserModel>> GetUser()
        {
            var model = await _userRepository.GetUser();
            return model == null ? NotFound() : Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserModel>> GetUser(Guid id)
        {
            var model = await _userRepository.GetUser(id);
            return model == null ? NotFound() : Ok(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<GetUserModel>> PostUser(PostUserModel postUserModel)
        {
            GetUserModel getUserModel = await _userRepository.PostUser(postUserModel, IpAddress());
            return CreatedAtAction("GetUser", new { id = getUserModel.Id }, getUserModel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetUserModel>> PutUser([FromRoute] Guid id, [FromBody] PutUserModel putUserModel)
        {
            GetUserModel model = await _userRepository.PutUser(id, putUserModel);
            return model != null ? Ok(model) : NotFound();
        }
    }
}
