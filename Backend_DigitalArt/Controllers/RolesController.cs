using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets a list of roles.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 123ms
        /// </remarks>
        /// <returns>A list of roles.</returns>
        [EnableCors("AllowAnyOrigins")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<GetRoleModel>>> GetRoles()
        {
            var models = await _roleRepository.GetRoles();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets the Exhibitor role.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 119ms
        /// </remarks>
        /// <returns>The Exhibitor role.</returns>
        [EnableCors("AllowAnyOrigins")]
        [AllowAnonymous]
        [HttpGet("Exhibitor")]
        public async Task<ActionResult<GetRoleModel>> GetRoleExhibitor()
        {
            var model = await _roleRepository.GetRoleExhibitor();
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets a role by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 127ms
        /// </remarks>
        /// <param name="id">The ID of the role.</param>
        /// <returns>A role object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetRoleModel>> GetRole(Guid id)
        {
            var model = await _roleRepository.GetRole(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 168ms
        /// </remarks>
        /// <param name="postRoleModel">The role to create.</param>
        /// <returns>The created role.</returns>
        [HttpPost]
        public async Task<ActionResult<GetRoleModel>> PostRole(PostRoleModel postRoleModel)
        {
            GetRoleModel getRoleModel = await _roleRepository.PostRole(postRoleModel);
            return CreatedAtAction("GetRole", new { id = getRoleModel.Id }, getRoleModel);
        }
    }
}
