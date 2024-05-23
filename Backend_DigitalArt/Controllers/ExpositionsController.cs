using Backend_DigitalAr.Services.Implementations;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Expositions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ExpositionsController : ControllerBase
    {
        private readonly ExpositionService _expositionService;
        private readonly IExpositionRepository _expositionRepository;

        public ExpositionsController(IExpositionRepository expositionRepository, ExpositionService expositionService)
        {
            _expositionRepository = expositionRepository;
            _expositionService = expositionService;
        }

        /// <summary>
        /// Gets a list of expositions.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 118ms
        /// </remarks>
        /// <returns>A list of expositions.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetExpositionModel>>> GetExpositions()
        {
            var models = await _expositionRepository.GetExpositions();
            return models == null ? NotFound() : Ok(models);
        }

        /*/// <summary>
        /// Partially updates an exposition.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 164ms
        /// </remarks>
        /// <param name="id">The ID of the exposition to update.</param>
        /// <param name="patchExpositionModel">The updated exposition data.</param>
        /// <returns>The updated exposition.</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<GetExpositionModel>> PatchExposition([FromRoute] Guid id, [FromBody] PatchExpositionModel patchExpositionModel)
        {
            var model = await _expositionRepository.PatchExposition(id, patchExpositionModel);
            await _expositionService.UpdateExpositionStatus(id, patchExpositionModel);
            return model == null ? NotFound() : Ok(model);
        }*/
    }
}
