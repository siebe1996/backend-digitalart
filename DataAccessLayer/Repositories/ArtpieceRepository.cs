using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Artists;
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

        //toDo fix forbiddenexceptions
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

        public async Task<List<GetArtpieceModel>> GetArtpiecesExposition(Guid expositionId)
        {
            List<GetArtpieceModel> artpieces = await _context.ExpositionArtpieces
                .Where(ea => ea.ExpositionId == expositionId)
                .Select(ea => new GetArtpieceModel
                {
                    Id = ea.Artpiece.Id,
                    ArtistId = ea.Artpiece.ArtistId,
                    Title = ea.Artpiece.Title,
                    Description = ea.Artpiece.Description,
                    ImageData = ea.Artpiece.ImageData,
                    MimeTypeImageData = ea.Artpiece.MimeTypeImageData,
                    Categories = ea.Artpiece.ArtpieceCategories.Select(ac => new GetCategoryModel
                    {
                        Id = ac.Category.Id,
                        Name = ac.Category.Name,
                        Description = ac.Category.Description,
                        CreatedAt = ac.Category.CreatedAt,
                        UpdatedAt = ac.Category.UpdatedAt,
                    }).ToList(),
                    CreatedAt = ea.Artpiece.CreatedAt,
                    UpdatedAt = ea.Artpiece.UpdatedAt,
                })
                .AsNoTracking()
                .ToListAsync();

            return artpieces;
        }

        public async Task<List<GetArtpieceExpandedModel>> GetArtpiecesUnrated()
        {
            Guid userId = new Guid(_user.Identity.Name);
            var artpiecesRated = _context.Likes
                                                 .Where(l => l.UserId == userId)
                                                 .Select(l => l.ArtpieceId)
                                                 .Distinct();

            List<GetArtpieceExpandedModel> artpieces = await _context.Artpieces
                .Where(a => !artpiecesRated.Contains(a.Id))
                .Select(x => new GetArtpieceExpandedModel
            {
                Id = x.Id,
                Artist = new GetArtistReducedModel
                {
                    Id = x.Artist.Id,
                    FirstName = x.Artist.FirstName,
                    LastName = x.Artist.LastName,
                },
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

        public async Task<List<GetArtpieceExpandedModel>> GetArtpiecesRatedLiked()
        {
            Guid userId = new Guid(_user.Identity.Name);

            List<GetArtpieceExpandedModel> artpieces = await _context.Artpieces
        .Where(ap => ap.Likes.Any(l => l.UserId == userId && l.Liked))
        .Select(ap => new GetArtpieceExpandedModel
        {
            Id = ap.Id,
            Artist = new GetArtistReducedModel
            {
                Id = ap.Artist.Id,
                FirstName = ap.Artist.FirstName,
                LastName = ap.Artist.LastName,
            },
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

        public async Task<List<GetArtpieceExpandedModel>> GetArtpiecesRatedLikedByArtistNames(string artistNames)
        {
            // Ensure the search terms are correctly split and converted to lowercase
            string[] searchTerms = artistNames.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Guid userId = new Guid(_user.Identity.Name);

            // Fetch artpieces that the user liked
            var likedArtpieces = await _context.Artpieces
                .Where(ap => ap.Likes.Any(l => l.UserId == userId && l.Liked))
                .Select(ap => new GetArtpieceExpandedModel
                {
                    Id = ap.Id,
                    Artist = new GetArtistReducedModel
                    {
                        FirstName = ap.Artist.FirstName,
                        LastName = ap.Artist.LastName,
                    },
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

            // Filter by artist names on the client side
            var filteredArtpieces = likedArtpieces
                .Where(ap => searchTerms.All(term => ap.Artist.FirstName.ToLower().Contains(term) ||
                                                     ap.Artist.LastName.ToLower().Contains(term)))
                .ToList();

            return filteredArtpieces;
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
