using Backend_DigitalArt.Services.Implementations;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artpieces;
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
    }
}
