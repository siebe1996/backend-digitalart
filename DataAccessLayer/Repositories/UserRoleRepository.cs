using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.UsersRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly Backend_DigitalArtContext _context;

        public UserRoleRepository(Backend_DigitalArtContext backend_DigitalArtContext, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = backend_DigitalArtContext;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<GetUserRoleModel>> GetUserRoles()
        {
            List<GetUserRoleModel> userRoles = await _context.UserRoles.Select(x => new GetUserRoleModel
            {
                UserId = x.UserId,
                RoleId = x.RoleId,
            }).AsNoTracking()
            .ToListAsync();

            return userRoles;
        }

        public async Task<List<GetUserRoleModel>> GetUserRolesByUserId(Guid id)
        {
            List<GetUserRoleModel> userRoles = await _context.UserRoles.Select(x => new GetUserRoleModel
            {
                UserId = x.UserId,
                RoleId = x.RoleId,
            }).AsNoTracking()
            .Where(x => x.UserId == id)
            .ToListAsync();

            return userRoles;
        }

        public async Task<List<GetUserRoleModel>> GetUserRolesByRoleId(Guid id)
        {
            List<GetUserRoleModel> userRoles = await _context.UserRoles.Select(x => new GetUserRoleModel
            {
                UserId = x.UserId,
                RoleId = x.RoleId,
            }).AsNoTracking()
            .Where(x => x.RoleId == id)
            .ToListAsync();

            return userRoles;
        }

        public async Task<GetUserRoleModel> GetUserRoleByIds(Guid userId, Guid roleId)
        {
            GetUserRoleModel userRole = await _context.UserRoles.Select(x => new GetUserRoleModel
            {
                UserId = x.UserId,
                RoleId = x.RoleId,
            }).AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);

            return userRole;
        }

        public async Task<GetUserRoleModel> AddUserToRole(PostUserRoleModel postUserRoleModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == postUserRoleModel.UserId);
            var userRoleModel = await this.GetUserRoleByIds(postUserRoleModel.UserId, postUserRoleModel.RoleId);
            if (user != null)
            {
                if (userRoleModel == null)
                {
                    String? roleName = await _context.Roles.Where(x => x.Id == postUserRoleModel.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
                    if (roleName != null)
                    {
                        IdentityResult result = _userManager.AddToRoleAsync(user, roleName).Result;
                        return new GetUserRoleModel { UserId = postUserRoleModel.UserId, RoleId = postUserRoleModel.RoleId };
                    }
                    else
                    {
                        throw new Exception("role not found");
                    }
                }
                else
                {
                    throw new Exception("user has role already");
                }
            }
            else
            {
                throw new Exception("user not found");
            }
        }
    }
}
