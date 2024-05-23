using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Expositions;
using Models.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ExpositionRepository : IExpositionRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        //toDo fix forbiddenexceptions

        public ExpositionRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetExpositionModel> GetExposition(Guid id)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var exposition = await _context.Expositions
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetExpositionModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.Active,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .FirstOrDefaultAsync();

            if (exposition == null)
            {
                throw new NotFoundException("Exposition Not Found");
            }
            return exposition;
        }

        public async Task<List<GetExpositionModel>> GetExpositions()
        {
            var expositions = await _context.Expositions.Select(x => new GetExpositionModel
            {
                Id = x.Id,
                Name = x.Name,
                Active = x.Active,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).AsNoTracking()
            .ToListAsync();

            return expositions;
        }

        public async Task<List<GetExpositionExpandedModel>> GetExpositionsMine()
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            Guid userId = new Guid(_user.Identity.Name);

            var expositions = await _context.ExhibitorPlaces
            .Where(ep => ep.ExhibitorId == userId)
            .SelectMany(ep => ep.Place.RentalAgreements)
            .Where(ra => ra.Projector != null && ra.Projector.Exposition != null) // Ensure Projector and Exposition are not null
            .Select(ra => ra.Projector.Exposition)
            .Distinct()
            .Select(e => new GetExpositionExpandedModel
            {
                Id = e.Id,
                Name = e.Name,
                Active = e.Active,
                CategoryIds = e.ExpositionCategories.Select(ec => ec.CategoryId).ToList(),
                RentalAgreementIds = e.Projectors.SelectMany(p => p.RentalAgreements).Select(ra => ra.Id).ToList(),
                ProjectorIds = e.Projectors.Select(p => p.Id).ToList(),
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .AsNoTracking()
            .ToListAsync();

            return expositions;
        }

        public async Task<List<Guid>> GetExpositionsIdsMineActive()
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            Guid userId = new Guid(_user.Identity.Name);
            DateTime now = DateTime.UtcNow;

            List<Guid> expositionsIds = await _context.ExhibitorPlaces
                .Where(ep => ep.ExhibitorId == userId)
                .SelectMany(ep => ep.Place.RentalAgreements)
                .Where(ra => ra.Projector != null && ra.Projector.Exposition != null && ra.StartDate <= now && ra.EndDate >= now)
                .Select(ra => ra.Projector.Exposition.Id)
                .Distinct()
                .ToListAsync();

            return expositionsIds;
        }


        public async Task<GetExpositionModel> PostExposition(PostExpositionModel postExpositionModel)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var exposition = new Exposition
            {
                Name = postExpositionModel.Name,
                Active = false,
            };

            var categoryIds = postExpositionModel.CategoryIds;
            var projectorIds = postExpositionModel.ProjectorIds;

            _context.Expositions.Add(exposition);
            await _context.SaveChangesAsync();

            foreach (var projectorId in projectorIds)
            {
                var projector = await _context.Projectors.FindAsync(projectorId);
                if (projector != null)
                {
                    projector.ExpositionId = exposition.Id;
                    _context.Projectors.Update(projector);
                }
            }

            await _context.SaveChangesAsync();

            var categoryArtpieces = _context.ArtpieceCategories
            .Where(ac => postExpositionModel.CategoryIds.Contains(ac.CategoryId))
            .Select(ac => new {
                Artpiece = ac.Artpiece,
                LikeCount = ac.Artpiece.Likes.Count(l => l.Liked),
                DislikeCount = ac.Artpiece.Likes.Count(l => !l.Liked)
            })
            .ToList();

            var weightedArtpieces = categoryArtpieces
            .GroupBy(x => x.Artpiece)
            .Select(g => new WeightedArtpiece
            {
                Id = g.Key.Id,
                Score = g.Sum(x => x.LikeCount) - g.Sum(x => x.DislikeCount) + g.Count()
            })
            .OrderByDescending(x => x.Score)
            .ToList();

            var selectedArtpiecesIds = HelperFunctions.WeightedRandomSelection(weightedArtpieces, 50);

            foreach (var id in selectedArtpiecesIds)
            {
                _context.ExpositionArtpieces.Add(new ExpositionArtpiece
                {
                    ExpositionId = exposition.Id,
                    ArtpieceId = id
                });
            }

            foreach (var categoryId in categoryIds)
            {
                _context.ExpositionCategories.Add(new ExpositionCategory
                {
                    ExpositionId = exposition.Id,
                    CategoryId = categoryId
                });
            }

            await _context.SaveChangesAsync();

            var getExpositionModel = new GetExpositionModel
            {
                Id = exposition.Id,
                Name = exposition.Name,
                Active = exposition.Active,
                CreatedAt = exposition.CreatedAt,
                UpdatedAt = exposition.UpdatedAt,
            };
            return getExpositionModel;
        }

        public async Task<GetExpositionModel> PutExposition(Guid id, PutExpositionModel putExpositionModel)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var exposition = await _context.Expositions
                .Include(e => e.Projectors)
                .Include(e => e.ExpositionCategories)
                .Include(e => e.ExpositionArtpieces)
                .SingleOrDefaultAsync(e => e.Id == id);

            if (exposition == null)
            {
                throw new NotFoundException("Exposition Not Found");
            }

            exposition.Name = putExpositionModel.Name;
            exposition.Active = putExpositionModel.Active;

            var categoryIds = putExpositionModel.CategoryIds;
            var projectorIds = putExpositionModel.ProjectorIds;

            // Update projectors
            var existingProjectors = exposition.Projectors.ToList();
            foreach (var projector in existingProjectors)
            {
                projector.ExpositionId = null;
                _context.Projectors.Update(projector);
            }

            foreach (var projectorId in projectorIds)
            {
                var projector = await _context.Projectors.FindAsync(projectorId);
                if (projector != null)
                {
                    projector.ExpositionId = exposition.Id;
                    _context.Projectors.Update(projector);
                }
            }

            // Update categories
            var existingCategories = exposition.ExpositionCategories.ToList();
            _context.ExpositionCategories.RemoveRange(existingCategories);

            foreach (var categoryId in categoryIds)
            {
                _context.ExpositionCategories.Add(new ExpositionCategory
                {
                    ExpositionId = exposition.Id,
                    CategoryId = categoryId
                });
            }

            // Update artpieces
            var categoryArtpieces = _context.ArtpieceCategories
                .Where(ac => putExpositionModel.CategoryIds.Contains(ac.CategoryId))
                .Select(ac => new {
                    Artpiece = ac.Artpiece,
                    LikeCount = ac.Artpiece.Likes.Count(l => l.Liked),
                    DislikeCount = ac.Artpiece.Likes.Count(l => !l.Liked)
                })
                .ToList();

            var weightedArtpieces = categoryArtpieces
                .GroupBy(x => x.Artpiece)
                .Select(g => new WeightedArtpiece
                {
                    Id = g.Key.Id,
                    Score = g.Sum(x => x.LikeCount) - g.Sum(x => x.DislikeCount) + g.Count()
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            var selectedArtpiecesIds = HelperFunctions.WeightedRandomSelection(weightedArtpieces, 50);

            var existingArtpieces = exposition.ExpositionArtpieces.ToList();
            _context.ExpositionArtpieces.RemoveRange(existingArtpieces);

            foreach (var selectedArtpiecesId in selectedArtpiecesIds)
            {
                _context.ExpositionArtpieces.Add(new ExpositionArtpiece
                {
                    ExpositionId = exposition.Id,
                    ArtpieceId = selectedArtpiecesId
                });
            }

            await _context.SaveChangesAsync();

            var getExpositionModel = new GetExpositionModel
            {
                Id = exposition.Id,
                Name = exposition.Name,
                Active = exposition.Active,
                CreatedAt = exposition.CreatedAt,
                UpdatedAt = exposition.UpdatedAt,
            };

            return getExpositionModel;
        }


        public async Task<GetExpositionModel> PatchExposition(Guid id, PatchExpositionModel patchExpostionModel)
        {
            var exposition = await _context.Expositions.FindAsync(id);
            if (exposition == null)
            {
                throw new NotFoundException("Exposition Not Found");
            }

            exposition.Active = patchExpostionModel.Active;

            _context.Expositions.Update(exposition);
            await _context.SaveChangesAsync();

            var getExpositionModel = new GetExpositionModel
            {
                Id = exposition.Id,
                Name = exposition.Name,
                Active = exposition.Active,
                CreatedAt = exposition.CreatedAt,
                UpdatedAt = exposition.UpdatedAt,
            };
            return getExpositionModel;
        }
    }
}
