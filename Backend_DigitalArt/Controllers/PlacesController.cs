using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Places;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceRepository _placeRepository;

        public PlacesController(IPlaceRepository placeRepository)
        {
            _placeRepository = placeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetPlaceModel>> GetPlaces()
        {
            var models = await _placeRepository.GetPlaces();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Mine")]
        public async Task<ActionResult<GetPlaceModel>> GetPlacesMine()
        {
            var models = await _placeRepository.GetPlacesMine();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlaceModel>> GetPlace(Guid Id)
        {
            var model = await _placeRepository.GetPlace(Id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetPlaceModel>> PostPlace(PostPlaceModel postPlaceModel)
        {
            var model = await _placeRepository.PostPlace(postPlaceModel);
            return CreatedAtAction("GetPlace", new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetPlaceModel>> PutPlace([FromRoute] Guid id, [FromBody] PutPlaceModel putPlaceModel)
        {
            var model = await _placeRepository.PutPlace(id, putPlaceModel);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<GetPlaceModel>> PatchPlace([FromRoute] Guid id, [FromBody] PatchPlaceModel patchPlaceModel)
        {
            var model = await _placeRepository.PatchPlace(id, patchPlaceModel);
            return model == null ? NotFound() : Ok(model);
        }
    }
}
