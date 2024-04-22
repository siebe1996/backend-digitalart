using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.UsersRoles;
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

        [HttpGet]
        public async Task<ActionResult<GetUserRoleModel>> GetUserRoles()
        {
            List<GetUserRoleModel> results = await _userRoleRepository.GetUserRoles();
            return results == null ? NotFound() : Ok(results);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<GetUserRoleModel>> GetUserRoleByIds(Guid userId, Guid roleId)
        {
            GetUserRoleModel result = await _userRoleRepository.GetUserRoleByIds(userId, roleId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GetUserRoleModel>> AddUserToRole(PostUserRoleModel postUserRoleModel)
        {
            GetUserRoleModel result = await _userRoleRepository.AddUserToRole(postUserRoleModel);
            return CreatedAtAction("GetUserRoleByIds", new { userId = result.UserId, roleId = result.RoleId }, result);
        }
    }
}
