using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Roles;
namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RolesController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<GetRoleModel>> GetRoles()
        {
            var models = await _roleRepository.GetRoles();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Exhibitor")]
        public async Task<ActionResult<GetRoleModel>> GetRoleExhibitor()
        {
            var model = await _roleRepository.GetRoleExhibitor();
            return model == null ? NotFound() : Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetRoleModel>> GetRole(Guid Id)
        {
            var model = await _roleRepository.GetRole(Id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetRoleModel>> PostRole(PostRoleModel postRoleModel)
        {
            GetRoleModel getRoleModel = await _roleRepository.PostRole(postRoleModel);
            return CreatedAtAction("GetRole", new { id = getRoleModel.Id }, getRoleModel);
        }
    }
}
