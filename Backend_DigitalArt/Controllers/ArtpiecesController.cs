using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artpieces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ArtpiecesController : ControllerBase
    {
        private readonly IArtpieceRepository _artpieceRepository;

        public ArtpiecesController(IArtpieceRepository artpieceRepository)
        {
            _artpieceRepository = artpieceRepository;
        }

        /// <summary>
        /// Gets a list of artpieces.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 142ms
        /// </remarks>
        /// <returns>A list of artpieces.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpieces()
        {
            var models = await _artpieceRepository.GetArtpieces();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of unrated artpieces.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 127ms
        /// </remarks>
        /// <returns>A list of unrated artpieces.</returns>
        [HttpGet("Unrated")]
        public async Task<ActionResult<List<GetArtpieceExpandedModel>>> GetArtpiecesUnrated()
        {
            var models = await _artpieceRepository.GetArtpiecesUnrated();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of unrated artpieces by category IDs.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 153ms
        /// </remarks>
        /// <param name="categoryIds">The list of category IDs to filter by.</param>
        /// <returns>A list of unrated artpieces filtered by category IDs.</returns>
        [HttpGet("Unrated/Search")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpiecesUnratedBySearch([FromQuery] List<Guid> categoryIds)
        {
            if (categoryIds != null && categoryIds.Count > 0)
            {
                var models = await _artpieceRepository.GetArtpiecesUnratedByCategoryIds(categoryIds);
                return models == null ? NotFound() : Ok(models);
            }
            else
            {
                return BadRequest("Please provide categoryIds.");
            }
        }

        /// <summary>
        /// Gets a list of liked and rated artpieces.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 138ms
        /// </remarks>
        /// <returns>A list of liked and rated artpieces.</returns>
        [HttpGet("Rated/Liked")]
        public async Task<ActionResult<List<GetArtpieceExpandedModel>>> GetArtpiecesRatedLiked()
        {
            var models = await _artpieceRepository.GetArtpiecesRatedLiked();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of liked and rated artpieces by artist names.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 162ms
        /// </remarks>
        /// <param name="artistNames">The names of the artists to filter by.</param>
        /// <returns>A list of liked and rated artpieces filtered by artist names.</returns>
        [HttpGet("Rated/Liked/Search")]
        public async Task<ActionResult<List<GetArtpieceExpandedModel>>> GetArtpiecesRatedLikedBySearch([FromQuery] string artistNames)
        {
            if (!string.IsNullOrEmpty(artistNames))
            {
                // Only artistName is provided
                var models = await _artpieceRepository.GetArtpiecesRatedLikedByArtistNames(artistNames);
                return models == null ? NotFound() : Ok(models);
            }
            else
            {
                var models = await _artpieceRepository.GetArtpiecesRatedLiked();
                return models == null ? NotFound() : Ok(models);
            }
        }

        /// <summary>
        /// Gets an artpiece by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 131ms
        /// </remarks>
        /// <param name="id">The ID of the artpiece.</param>
        /// <returns>An artpiece object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetArtpieceModel>> GetArtpiece(Guid id)
        {
            var model = await _artpieceRepository.GetArtpiece(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new artpiece.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 184ms
        /// </remarks>
        /// <param name="postArtpieceModel">The artpiece to create.</param>
        /// <returns>The created artpiece.</returns>
        [HttpPost]
        public async Task<ActionResult<GetArtpieceModel>> PostArtpiece(PostArtpieceModel postArtpieceModel)
        {
            var model = await _artpieceRepository.PostArtpiece(postArtpieceModel);
            return CreatedAtAction("GetArtpiece", new { id = model.Id }, model);
        }
    }
}
