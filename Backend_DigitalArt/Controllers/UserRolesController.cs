using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.UsersRoles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRolesController(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// Gets a list of user roles.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 133ms
        /// </remarks>
        /// <returns>A list of user roles.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetUserRoleModel>>> GetUserRoles()
        {
            List<GetUserRoleModel> results = await _userRoleRepository.GetUserRoles();
            return results == null ? NotFound() : Ok(results);
        }

        /// <summary>
        /// Gets a user role by user ID and role ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 129ms
        /// </remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A user role object.</returns>
        [HttpGet("Search")]
        public async Task<ActionResult<GetUserRoleModel>> GetUserRoleByIds(Guid userId, Guid roleId)
        {
            GetUserRoleModel result = await _userRoleRepository.GetUserRoleByIds(userId, roleId);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 174ms
        /// </remarks>
        /// <param name="postUserRoleModel">The user role to create.</param>
        /// <returns>The created user role.</returns>
        [HttpPost]
        public async Task<ActionResult<GetUserRoleModel>> AddUserToRole(PostUserRoleModel postUserRoleModel)
        {
            GetUserRoleModel result = await _userRoleRepository.AddUserToRole(postUserRoleModel);
            return CreatedAtAction("GetUserRoleByIds", new { userId = result.UserId, roleId = result.RoleId }, result);
        }
    }
}
