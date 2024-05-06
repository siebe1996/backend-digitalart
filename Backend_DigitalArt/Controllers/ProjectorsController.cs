using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Projectors;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ProjectorsController : ControllerBase
    {
        //toDo fix admin routes
        private readonly IProjectorRepository _projectorRepository;

        public ProjectorsController(IProjectorRepository projectorRepository)
        {
            _projectorRepository = projectorRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetProjectorModel>> GetProjectors()
        {
            var models = await _projectorRepository.GetProjectors();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProjectorModel>> GetProjector(Guid Id)
        {
            var model = await _projectorRepository.GetProjector(Id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetProjectorModel>> PostProjector(PostProjectorModel postProjectorModel)
        {
            var model = await _projectorRepository.PostProjector(postProjectorModel);
            return CreatedAtAction("GetProjector", new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetProjectorModel>> PutProjector([FromRoute] Guid id, [FromBody] PutProjectorModel putProjectorModel)
        {
            var model = await _projectorRepository.PutProjector(id, putProjectorModel);
            return model != null ? Ok(model) : NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<GetProjectorModel>> PatchProjector([FromRoute] Guid id, [FromBody] PatchProjectorModel patchProjectorModel)
        {
            var model = await _projectorRepository.PatchProjector(id, patchProjectorModel);
            return model != null ? Ok(model) : NotFound();
        }
    }
}
