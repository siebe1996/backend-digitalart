using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Artists;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly Backend_DigitalArtContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArtistRepository(Backend_DigitalArtContext backend_DigitalArtContext, UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = backend_DigitalArtContext;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<List<GetArtistModel>> GetArtists()
        {
            // Query the Artists table directly
            List<GetArtistModel> artists = await _context.Artists.Select(artist => new GetArtistModel
            {
                Id = artist.Id,
                FirstName = artist.FirstName,
                LastName = artist.LastName,
                DateOfBirth = artist.DateOfBirth,
                Email = artist.Email,
                ImageData = artist.ImageData,
                MimeTypeImageData = artist.MimeTypeImageData,
                Description = artist.Description,
                Country = artist.Country,
                Province = artist.Province,
                City = artist.City,
                PostalCode = artist.PostalCode,
                Street = artist.Street,
                Address = artist.Address,
                CreatedAt = artist.CreatedAt,
                UpdatedAt = artist.UpdatedAt
            }).AsNoTracking()
            .ToListAsync();

            return artists;
        }

        public async Task<GetArtistModel> GetArtist(Guid id)
        {
            var user = await _context.Artists.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new NotFoundException("Not Found");
            }
            var roleNames = await _userManager.GetRolesAsync(user);

            return new GetArtistModel
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
                Description = user.Description,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<GetArtistModel> GetArtist()
        {
            bool hasAccessArtist = _user.IsInRole("Artist");
            if (hasAccessArtist)
            {
                Guid userId = new Guid(_user.Identity.Name);
                if(userId == Guid.Empty)
                {
                    throw new NotFoundException("Not Found");
                }
                var artist = await _context.Artists.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
                if (artist == null)
                {
                    throw new NotFoundException("Not Found");
                }
                var roleNames = await _userManager.GetRolesAsync(artist);
                GetArtistModel artistModel = new GetArtistModel
                {
                    Id = artist.Id,
                    FirstName = artist.FirstName,
                    LastName = artist.LastName,
                    DateOfBirth = artist.DateOfBirth,
                    Email = artist.Email,
                    ImageData = artist.ImageData,
                    MimeTypeImageData = artist.MimeTypeImageData,
                    Description = artist.Description,
                    Country = artist.Country,
                    Province = artist.Province,
                    City = artist.City,
                    PostalCode = artist.PostalCode,
                    Street = artist.Street,
                    Address = artist.Address,
                    Roles = roleNames,
                    CreatedAt = artist.CreatedAt,
                    UpdatedAt = artist.UpdatedAt
                };

                return artistModel;
            }
            else
            {
                throw new ForbiddenException("Not allowed"); 
            } 
        }

        public async Task<GetArtistModel> PostArtist(PostArtistModel postArtistModel, string ipAddress)
        {
            Artist user = new Artist();

            user.FirstName = postArtistModel.FirstName;
            user.LastName = postArtistModel.LastName;
            user.DateOfBirth = postArtistModel.DateOfBirth;
            user.Email = postArtistModel.Email;
            user.ImageData = postArtistModel.ImageData;
            user.MimeTypeImageData = postArtistModel.MimeTypeImageData;
            user.UserName = postArtistModel.Email;
            user.Country = postArtistModel.Country;
            user.Province = postArtistModel.Province;
            user.City = postArtistModel.City;
            user.PostalCode = postArtistModel.PostalCode;
            user.Street = postArtistModel.Street;
            user.Address = postArtistModel.Address;
            user.Description = postArtistModel.Description;

            var createResult = await _userManager.CreateAsync(user, postArtistModel.Password);
            if (!createResult.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Artist");
            if (!roleResult.Succeeded)
            {
                // Handle failure: possibly throw an exception or return an error response
                throw new Exception($"Failed to add user to role Artist: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }


            await _context.SaveChangesAsync();
            var roleNames = await _userManager.GetRolesAsync(user);
            return new GetArtistModel
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
                Description = user.Description,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
