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
