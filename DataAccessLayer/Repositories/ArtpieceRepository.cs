using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Artpieces;
using Models.Categories;
using Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ArtpieceRepository : IArtpieceRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public ArtpieceRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetArtpieceModel> GetArtpiece(Guid id)
        {
            var artpiece = await _context.Artpieces
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetArtpieceModel
                {
                    Id = x.Id,
                    ArtistId = x.ArtistId,
                    ImageData = x.ImageData,
                    Title = x.Title,
                    Description = x.Description,
                    Categories = x.ArtpieceCategories.Select(x => new GetCategoryModel
                    {
                        Id = x.Category.Id,
                        Name = x.Category.Name,
                        Description = x.Category.Description,
                        CreatedAt = x.Category.CreatedAt,
                        UpdatedAt = x.Category.UpdatedAt,
                    }).ToList(),
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .FirstOrDefaultAsync();
            if (artpiece == null)
            {
                throw new NotFoundException("Artpiece Not Found");
            }
            return artpiece;
        }

        public async Task<GetArtpieceModel> PostArtpiece(PostArtpieceModel postArtpieceModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            var artpiece = new Artpiece
            {
                ArtistId = userId,
                ImageData = postArtpieceModel.ImageData,
                Title = postArtpieceModel.Title,
                Description = postArtpieceModel.Description,
                ArtpieceCategories = new List<ArtpieceCategory>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            foreach (var categoryId in postArtpieceModel.CategoryIds)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category == null)
                {
                    throw new ArgumentException($"Category with id '{categoryId}' does not exist.");
                }
                artpiece.ArtpieceCategories.Add(new ArtpieceCategory { CategoryId = categoryId, ArtpieceId = artpiece.Id });
            }

            /*foreach (var categoryName in postArtpieceModel.CategorieNames)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category == null)
                {
                    throw new ArgumentException($"Category with name '{categoryName}' does not exist.");
                }
                artpiece.ArtpieceCategories.Add(new ArtpieceCategory { Category = category, Artpiece = artpiece });
            }*/

            _context.Artpieces.Add(artpiece);
            await _context.SaveChangesAsync();

            var getArtpieceModel = new GetArtpieceModel
            {
                Id = artpiece.Id,
                ArtistId = artpiece.ArtistId,
                ImageData = artpiece.ImageData,
                Title = artpiece.Title,
                Description = artpiece.Description,
                Categories = artpiece.ArtpieceCategories.Select(x => new GetCategoryModel
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name,
                    Description = x.Category.Description,
                    CreatedAt = x.Category.CreatedAt,
                    UpdatedAt = x.Category.UpdatedAt,
                }).ToList(),
                CreatedAt = artpiece.CreatedAt,
                UpdatedAt = artpiece.UpdatedAt,
            };
            return getArtpieceModel;
        }
    }
}
