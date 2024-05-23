using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Likes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;

        public LikesController(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        /// <summary>
        /// Gets a list of likes.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 128ms
        /// </remarks>
        /// <returns>A list of likes.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetLikeModel>>> GetLikes()
        {
            var models = await _likeRepository.GetLikes();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets the authenticated user's likes.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 115ms
        /// </remarks>
        /// <returns>The authenticated user's likes.</returns>
        [HttpGet("Mine")]
        public async Task<ActionResult<GetLikeModel>> GetLikesMine()
        {
            var models = await _likeRepository.GetLikesMine();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a like for a specific artpiece.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 122ms
        /// </remarks>
        /// <param name="ArtpieceId">The ID of the artpiece.</param>
        /// <returns>A like object.</returns>
        [HttpGet("{ArtpieceId}")]
        public async Task<ActionResult<GetLikeModel>> GetLike(Guid ArtpieceId)
        {
            var model = await _likeRepository.GetLike(ArtpieceId);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new like.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 179ms
        /// </remarks>
        /// <param name="postLikeModel">The like to create.</param>
        /// <returns>The created like.</returns>
        [HttpPost]
        public async Task<ActionResult<GetLikeModel>> PostLike(PostLikeModel postLikeModel)
        {
            var model = await _likeRepository.PostLike(postLikeModel);
            return CreatedAtAction("GetLike", new { artpieceId = model.ArtpieceId }, model);
        }
    }
}
