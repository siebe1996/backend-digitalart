using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artists;
using Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistRepository _repository;

        public ArtistsController(IArtistRepository repository)
        {
            _repository = repository;
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
        /// Gets a list of artists.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 112ms
        /// </remarks>
        /// <returns>A list of artists.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetArtistModel>>> GetArtists()
        {
            var models = await _repository.GetArtists();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets the authenticated artist.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 89ms
        /// </remarks>
        /// <returns>The authenticated artist.</returns>
        [HttpGet("Me")]
        public async Task<ActionResult<GetArtistModel>> GetArtist()
        {
            var model = await _repository.GetArtist();
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets an artist by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 137ms
        /// </remarks>
        /// <param name="id">The ID of the artist.</param>
        /// <returns>An artist object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetArtistModel>> GetArtist(Guid id)
        {
            var model = await _repository.GetArtist(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new artist.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 221ms
        /// </remarks>
        /// <param name="postArtistModel">The artist to create.</param>
        /// <returns>The created artist.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<GetArtistModel>> PostArtist(PostArtistModel postArtistModel)
        {
            var model = await _repository.PostArtist(postArtistModel, IpAddress());
            return CreatedAtAction("GetArtist", new { id = model.Id }, model);
        }
    }
}
