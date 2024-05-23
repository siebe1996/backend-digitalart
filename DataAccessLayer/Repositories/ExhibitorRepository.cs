using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Artists;
using Models.Exhibitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ExhibitorRepository : IExhibitorRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly Backend_DigitalArtContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExhibitorRepository(Backend_DigitalArtContext backend_DigitalArtContext, UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = backend_DigitalArtContext;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<List<GetExhibitorModel>> GetExhibitors()
        {
            var models = await _context.Exhibitors.Select(x => new GetExhibitorModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                ImageData = x.ImageData,
                MimeTypeImageData = x.MimeTypeImageData,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Street = x.Street,
                Address = x.Address,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .ToListAsync();

            return models;
        }

        public async Task<GetExhibitorModel> GetExhibitor(Guid id)
        {
            var user = await _context.Exhibitors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new NotFoundException("Not Found");
            }
            var roleNames = await _userManager.GetRolesAsync(user);

            return new GetExhibitorModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<GetExhibitorModel> GetExhibitor()
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not allowed"); 
            }
                Guid userId = new Guid(_user.Identity.Name);
                if (userId == Guid.Empty)
                {
                    throw new NotFoundException("Not Found");
                }
                var exhibitor = await _context.Exhibitors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (exhibitor == null)
            {
                throw new NotFoundException("Not Found");
            }
            var roleNames = await _userManager.GetRolesAsync(exhibitor);
                var model = new GetExhibitorModel
                {
                    Id = exhibitor.Id,
                    FirstName = exhibitor.FirstName,
                    LastName = exhibitor.LastName,
                    DateOfBirth = exhibitor.DateOfBirth,
                    Email = exhibitor.Email,
                    ImageData = exhibitor.ImageData,
                    MimeTypeImageData = exhibitor.MimeTypeImageData,
                    Country = exhibitor.Country,
                    Province = exhibitor.Province,
                    City = exhibitor.City,
                    PostalCode = exhibitor.PostalCode,
                    Street = exhibitor.Street,
                    Address = exhibitor.Address,
                    Roles = roleNames,
                    CreatedAt = exhibitor.CreatedAt,
                    UpdatedAt = exhibitor.UpdatedAt
                };

                return model;
        }

        public async Task<GetExhibitorModel> PostExhibitor(PostExhibitorModel postModel, string ipAddress)
        {

            var user = new Exhibitor();

            user.FirstName = postModel.FirstName;
            user.LastName = postModel.LastName;
            user.DateOfBirth = postModel.DateOfBirth;
            user.Email = postModel.Email;
            user.ImageData = postModel.ImageData;
            user.MimeTypeImageData = postModel.MimeTypeImageData;
            user.UserName = postModel.Email;
            user.Country = postModel.Country;
            user.Province = postModel.Province;
            user.City = postModel.City;
            user.PostalCode = postModel.PostalCode;
            user.Street = postModel.Street;
            user.Address = postModel.Address;

            var createResult = await _userManager.CreateAsync(user, postModel.Password);
            if (!createResult.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Exhibitor");
            if (!roleResult.Succeeded)
            {
                // Handle failure: possibly throw an exception or return an error response
                throw new Exception($"Failed to add user to role Exhibitor: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }


            await _context.SaveChangesAsync();
            var roleNames = await _userManager.GetRolesAsync(user);
            return new GetExhibitorModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
