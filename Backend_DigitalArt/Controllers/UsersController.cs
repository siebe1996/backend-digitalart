using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // The refresh token is being sent in an HTTP only cookie and header
        private void SetTokenCookie(string token)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(131490), // TOKEN REFRESH
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

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 157ms
        /// </remarks>
        /// <param name="postAuthenticateRequestModel">The authentication request model.</param>
        /// <returns>The authentication response model.</returns>
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

        /// <summary>
        /// Renews the authentication token.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 163ms
        /// </remarks>
        /// <returns>The authentication response model.</returns>
        [AllowAnonymous]
        [HttpPost("Renew-token")]
        public async Task<ActionResult<PostAuthenticateResponseModel>> RenewToken()
        {
            try
            {
                string refreshToken = Request.Cookies["Backend_DigitalArt.RefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest("Refresh token is missing");
                }
                PostAuthenticateResponseModel postAuthenticateResponseModel = await _userRepository.RenewToken(refreshToken, IpAddress());
                SetTokenCookie(postAuthenticateResponseModel.RefreshToken);
                return Ok(postAuthenticateResponseModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deactivates a token.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 135ms
        /// </remarks>
        /// <param name="postDeactivateTokenRequestModel">The token deactivation request model.</param>
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

        /// <summary>
        /// Gets a list of users.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 121ms
        /// </remarks>
        /// <returns>A list of users.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetUserModel>>> GetUsers()
        {
            var models = await _userRepository.GetUsers();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets the authenticated user.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 117ms
        /// </remarks>
        /// <returns>The authenticated user.</returns>
        [HttpGet("me")]
        public async Task<ActionResult<GetUserModel>> GetUser()
        {
            var model = await _userRepository.GetUser();
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 123ms
        /// </remarks>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A user object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserModel>> GetUser(Guid id)
        {
            var model = await _userRepository.GetUser(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 171ms
        /// </remarks>
        /// <param name="postUserModel">The user to create.</param>
        /// <returns>The created user.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<GetUserModel>> PostUser(PostUserModel postUserModel)
        {
            GetUserModel getUserModel = await _userRepository.PostUser(postUserModel, IpAddress());
            return CreatedAtAction("GetUser", new { id = getUserModel.Id }, getUserModel);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 145ms
        /// </remarks>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="putUserModel">The updated user data.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<GetUserModel>> PutUser([FromRoute] Guid id, [FromBody] PutUserModel putUserModel)
        {
            GetUserModel model = await _userRepository.PutUser(id, putUserModel);
            return model != null ? Ok(model) : NotFound();
        }
    }
}
