using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(Backend_DigitalArtContext backend_DigitalArtContext, IHttpContextAccessor httpContextAccessor, RoleManager<Role> roleManager)
        {
            _context = backend_DigitalArtContext;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
            _roleManager = roleManager;
        }

        public async Task<List<GetRoleModel>> GetRoles()
        {
            List<GetRoleModel> roles = await _context.Roles
                .Where(x => x.Name != "Admin") // Exclude the "Admin" role
                .Select(x => new GetRoleModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .AsNoTracking()
                .ToListAsync();

            return roles;
        }


        public async Task<List<GetRoleModel>> GetAllRoles()
        {
            //toDo make change so only user with admin privilidge can get
            List<GetRoleModel> roles = await _context.Roles.Select(x => new GetRoleModel
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).AsNoTracking()
            .ToListAsync();

            return roles;
        }

        public async Task<GetRoleModel> GetRole(Guid id)
        {
            GetRoleModel role = await _context.Roles.Select(x => new GetRoleModel
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

            return role;
        }

        public async Task<GetRoleModel> GetRoleExhibitor()
        {
            GetRoleModel role = await _context.Roles
                .Select(x => new GetRoleModel
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == "Exhibitor");

            return role;
        }

        public async Task<GetRoleModel> PostRole(PostRoleModel postRoleModel)
        {
            Role role = new Role
            {
                Name = postRoleModel.Name,
                Description = postRoleModel.Description,
            };

            IdentityResult result = await _roleManager.CreateAsync(role);

            GetRoleModel roleModel = new GetRoleModel
            {
                Id = role.Id,
                Name = postRoleModel.Name,
                Description = role.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            return roleModel;
        }
    }
}
