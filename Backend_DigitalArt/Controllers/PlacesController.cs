using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Places;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets a list of places.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 126ms
        /// </remarks>
        /// <returns>A list of places.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetPlaceModel>>> GetPlaces()
        {
            var models = await _placeRepository.GetPlaces();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of places with active expositions.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 132ms
        /// </remarks>
        /// <returns>A list of places with active expositions.</returns>
        [HttpGet("Expositions/Active")]
        public async Task<ActionResult<List<GetPlaceModel>>> GetPlacesWithActiveExpositions()
        {
            var models = await _placeRepository.GetPlacesWithActiveExpositions();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Partially updates a place.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 143ms
        /// </remarks>
        /// <param name="id">The ID of the place to update.</param>
        /// <param name="patchPlaceModel">The updated place data.</param>
        /// <returns>The updated place.</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<GetPlaceModel>> PatchPlace([FromRoute] Guid id, [FromBody] PatchPlaceModel patchPlaceModel)
        {
            var model = await _placeRepository.PatchPlace(id, patchPlaceModel);
            return model == null ? NotFound() : Ok(model);
        }
    }
}
