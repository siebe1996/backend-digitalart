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
                    MimeTypeImageData = x.MimeTypeImageData,
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

        public async Task<List<GetArtpieceModel>> GetArtpieces()
        {
            List<GetArtpieceModel> artpieces = await _context.Artpieces.Select(x => new GetArtpieceModel
            {
                Id = x.Id,
                ArtistId = x.ArtistId,
                Title = x.Title,
                Description = x.Description,
                ImageData = x.ImageData,
                MimeTypeImageData = x.MimeTypeImageData,
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
            }).AsNoTracking()
            .ToListAsync();

            return artpieces;
        }

        public async Task<List<GetArtpieceModel>> GetArtpiecesUnrated()
        {
            Guid userId = new Guid(_user.Identity.Name);
            var artpiecesRated = _context.Likes
                                                 .Where(l => l.UserId == userId)
                                                 .Select(l => l.ArtpieceId)
                                                 .Distinct();

            List<GetArtpieceModel> artpieces = await _context.Artpieces
                .Where(a => !artpiecesRated.Contains(a.Id))
                .Select(x => new GetArtpieceModel
            {
                Id = x.Id,
                ArtistId = x.ArtistId,
                Title = x.Title,
                Description = x.Description,
                ImageData = x.ImageData,
                MimeTypeImageData = x.MimeTypeImageData,
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
            }).AsNoTracking()
            .ToListAsync();

            return artpieces;
        }

        public async Task<List<GetArtpieceModel>> GetArtpiecesUnratedByCategoryIds(List<Guid> categoryIds)
        {
            var artpieceIds = await _context.Artpieces
                .Where(x => x.ArtpieceCategories.Any(ac => categoryIds.Contains(ac.Category.Id)))
                .Select(x => x.Id)
                .Distinct()
                .ToListAsync();

            List<GetArtpieceModel> artpieces = await _context.Artpieces
                .Where(x => artpieceIds.Contains(x.Id))
                .Select(x => new GetArtpieceModel
                {
                    Id = x.Id,
                    ArtistId = x.ArtistId,
                    Title = x.Title,
                    Description = x.Description,
                    ImageData = x.ImageData,
                    MimeTypeImageData = x.MimeTypeImageData,
                    Categories = x.ArtpieceCategories.Select(ac => new GetCategoryModel
                    {
                        Id = ac.Category.Id,
                        Name = ac.Category.Name,
                        Description = ac.Category.Description,
                        CreatedAt = ac.Category.CreatedAt,
                        UpdatedAt = ac.Category.UpdatedAt,
                    }).ToList(),
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .AsNoTracking()
                .ToListAsync();

            return artpieces;
        }

        public async Task<List<GetArtpieceModel>> GetArtpiecesRatedLiked()
        {
            Guid userId = new Guid(_user.Identity.Name);

            List<GetArtpieceModel> artpieces = await _context.Artpieces
        .Where(ap => ap.Likes.Any(l => l.UserId == userId && l.Liked))
        .Select(ap => new GetArtpieceModel
        {
            Id = ap.Id,
            ArtistId = ap.ArtistId,
            Title = ap.Title,
            Description = ap.Description,
            ImageData = ap.ImageData,
            MimeTypeImageData = ap.MimeTypeImageData,
            Categories = ap.ArtpieceCategories.Select(ac => new GetCategoryModel
            {
                Id = ac.Category.Id,
                Name = ac.Category.Name,
                Description = ac.Category.Description,
                CreatedAt = ac.Category.CreatedAt,
                UpdatedAt = ac.Category.UpdatedAt,
            }).ToList(),
            CreatedAt = ap.CreatedAt,
            UpdatedAt = ap.UpdatedAt,
        })
        .AsNoTracking()
        .ToListAsync();

            return artpieces;
        }

        public async Task<List<GetArtpieceModel>> GetArtpiecesRatedLikedByArtistNames(string artistNames)
        {
            //toDo change so they can also search on title and its only the ones they liked
            string[] searchTerms = artistNames.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Guid userId = new Guid(_user.Identity.Name);

            List<GetArtpieceModel> artpieces = await _context.Artpieces
        .Where(ap => ap.Likes.Any(l => l.UserId == userId && l.Liked) // Checks that there's a like by this user that is true
                    && searchTerms.All(term => ap.Artist.FirstName.ToLower().Contains(term) ||
                                               ap.Artist.LastName.ToLower().Contains(term)))
        .Select(ap => new GetArtpieceModel
        {
            Id = ap.Id,
            ArtistId = ap.ArtistId,
            Title = ap.Title,
            Description = ap.Description,
            ImageData = ap.ImageData,
            MimeTypeImageData = ap.MimeTypeImageData,
            Categories = ap.ArtpieceCategories.Select(ac => new GetCategoryModel
            {
                Id = ac.Category.Id,
                Name = ac.Category.Name,
                Description = ac.Category.Description,
                CreatedAt = ac.Category.CreatedAt,
                UpdatedAt = ac.Category.UpdatedAt,
            }).ToList(),
            CreatedAt = ap.CreatedAt,
            UpdatedAt = ap.UpdatedAt,
        })
        .AsNoTracking()
        .ToListAsync();

            return artpieces;
        }


        public async Task<GetArtpieceModel> PostArtpiece(PostArtpieceModel postArtpieceModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            var artpiece = new Artpiece
            {
                ArtistId = userId,
                ImageData = postArtpieceModel.ImageData,
                MimeTypeImageData = postArtpieceModel.MimeTypeImageData,
                Title = postArtpieceModel.Title,
                Description = postArtpieceModel.Description,
                ArtpieceCategories = new List<ArtpieceCategory>(),
            };

            _context.Artpieces.Add(artpiece);
            await _context.SaveChangesAsync();

            foreach (var categoryId in postArtpieceModel.CategoryIds)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category == null)
                {
                    throw new ArgumentException($"Category with id '{categoryId}' does not exist.");
                }
                artpiece.ArtpieceCategories.Add(new ArtpieceCategory { CategoryId = categoryId, ArtpieceId = artpiece.Id });
            }

            await _context.SaveChangesAsync();

            var getArtpieceModel = new GetArtpieceModel
            {
                Id = artpiece.Id,
                ArtistId = artpiece.ArtistId,
                ImageData = artpiece.ImageData,
                MimeTypeImageData = artpiece.MimeTypeImageData,
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
