using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artpieces;

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

        [HttpGet]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpieces()
        {
            var models = await _artpieceRepository.GetArtpieces();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Unrated")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpiecesUnrated()
        {
            var models = await _artpieceRepository.GetArtpiecesUnrated();
            return models == null ? NotFound() : Ok(models);
        }

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

        [HttpGet("Rated/Liked")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpiecesRatedLiked()
        {
            var models = await _artpieceRepository.GetArtpiecesRatedLiked();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Rated/Liked/Search")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpiecesRatedLikedBySearch([FromQuery] string artistNames)
        {
            if (!string.IsNullOrEmpty(artistNames))
            {
                // Only artistName is provided
                var models = await _artpieceRepository.GetArtpiecesRatedLikedByArtistNames(artistNames);
                return models == null ? NotFound() : Ok(models);
            }
            else
            {
                return BadRequest("Please provide artistNames.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetArtpieceModel>> GetArtpiece(Guid id)
        {
            var model = await _artpieceRepository.GetArtpiece(id);
            return model == null ? NotFound() : Ok(model);
        }


        [HttpPost]
        public async Task<ActionResult<GetArtpieceModel>> PostArtpiece(PostArtpieceModel postArtpieceModel)
        {
            var model = await _artpieceRepository.PostArtpiece(postArtpieceModel);
            return CreatedAtAction("GetArtpiece", new {id =  model.Id}, model);
        }
    }
}
