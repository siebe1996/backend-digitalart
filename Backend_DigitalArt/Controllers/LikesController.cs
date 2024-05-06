using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Likes;
using Models.Roles;

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

        [HttpGet]
        public async Task<ActionResult<GetLikeModel>> GetLikes()
        {
            var models = await _likeRepository.GetLikes();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Mine")]
        public async Task<ActionResult<GetLikeModel>> GetLikesMine()
        {
            var models = await _likeRepository.GetLikesMine();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{ArtpieceId}")]
        public async Task<ActionResult<GetLikeModel>> GetLike(Guid ArtpieceId)
        {
            var model = await _likeRepository.GetLike(ArtpieceId);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetLikeModel>> PostLike(PostLikeModel postLikeModel) 
        {
            var model = await _likeRepository.PostLike(postLikeModel);
            return CreatedAtAction("GetLike", new { artpieceId = model.ArtpieceId }, model);
        }
    }
}
