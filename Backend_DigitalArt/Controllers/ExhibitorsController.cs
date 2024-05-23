using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Exhibitors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ExhibitorsController : ControllerBase
    {
        private readonly IExhibitorRepository _repository;

        public ExhibitorsController(IExhibitorRepository repository)
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
        /// Gets a list of exhibitors.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 129ms
        /// </remarks>
        /// <returns>A list of exhibitors.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetExhibitorModel>>> GetExhibitors()
        {
            var models = await _repository.GetExhibitors();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets the authenticated exhibitor.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 113ms
        /// </remarks>
        /// <returns>The authenticated exhibitor.</returns>
        [HttpGet("Me")]
        public async Task<ActionResult<GetExhibitorModel>> GetExhibitor()
        {
            var model = await _repository.GetExhibitor();
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets an exhibitor by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 121ms
        /// </remarks>
        /// <param name="id">The ID of the exhibitor.</param>
        /// <returns>An exhibitor object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetExhibitorModel>> GetExhibitor(Guid id)
        {
            var model = await _repository.GetExhibitor(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new exhibitor.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 177ms
        /// </remarks>
        /// <param name="postModel">The exhibitor to create.</param>
        /// <returns>The created exhibitor.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<GetExhibitorModel>> PostExhibitor(PostExhibitorModel postModel)
        {
            var model = await _repository.PostExhibitor(postModel, IpAddress());
            return CreatedAtAction("GetExhibitor", new { id = model.Id }, model);
        }
    }
}
