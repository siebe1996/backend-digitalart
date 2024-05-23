using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Places;
using Models.Projectors;
using Models.RentalAgreements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class RentalAgreementRepository : IRentalAgreementRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public RentalAgreementRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetRentalAgreementModel> GetRentalAgreement(Guid id)
        {

            Guid userId = new Guid(_user.Identity.Name);
            var rentalAgreement = await _context.RentalAgreements
                .Include(ra => ra.Place)
                .ThenInclude(p => p.ExhibitorPlaces)
                .SingleOrDefaultAsync(ra => ra.Id == id);

            bool hasAccessAdmin = _user.IsInRole("Admin");

            if (rentalAgreement == null) { 
                if (hasAccessAdmin)
                {
                    throw new NotFoundException("Rental agreement not found.");
                }
                else
                {
                    throw new ForbiddenException("Forbidden to access this page");
                }
                
            }

            bool hasAccessExhibitor = rentalAgreement.Place.ExhibitorPlaces.Any(ep => ep.ExhibitorId == userId) && _user.IsInRole("Exhibitor");

            if (!(hasAccessAdmin || hasAccessExhibitor))
            {
                throw new ForbiddenException("Forbidden to access this page");
            }

            var model = new GetRentalAgreementModel
            {
                Id = rentalAgreement.Id,
                ProjectorId = rentalAgreement.ProjectorId,
                StartDate = rentalAgreement.StartDate,
                EndDate = rentalAgreement.EndDate,
                CreatedAt = rentalAgreement.CreatedAt,
                UpdatedAt = rentalAgreement.UpdatedAt,
            };

            return model;
        }

        public async Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreements()
        {
            bool hasAccessAdmin = _user.IsInRole("Admin");

            if (!hasAccessAdmin)
            {
                throw new ForbiddenException("Forbidden to access this page");
            }

            List<GetRentalAgreementExpandedModel> rentalAgreements = await _context.RentalAgreements
                .Select(x => new GetRentalAgreementExpandedModel
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Projector = new GetProjectorModel
                    {
                        Id = x.Projector.Id,
                        Model = x.Projector.Model,
                        Brand = x.Projector.Brand,
                        SerialNumber = x.Projector.SerialNumber,
                        Damages = x.Projector.Damages,
                        Remarks = x.Projector.Remarks,
                        ExpositionId = x.Projector.ExpositionId,
                        Available = x.Projector.Available,
                        CreatedAt = x.Projector.CreatedAt,
                        UpdatedAt = x.Projector.UpdatedAt,
                    },
                    Place = new GetPlaceReducedModel
                    {
                        Id = x.Place.Id,
                        Name = x.Place.Name,
                        Description = x.Place.Description,
                        Address = x.Place.Address,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.CreatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                }).AsNoTracking()
            .ToListAsync();

            return rentalAgreements;
        }

        public async Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreementsMine()
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Forbidden to access this page");
            }
            Guid userId = new Guid(_user.Identity.Name);

            List<GetRentalAgreementExpandedModel> rentalAgreements = await _context.RentalAgreements
                .Where(x => x.Place.ExhibitorPlaces.Any(ep => ep.ExhibitorId == userId))
                .Select(x => new GetRentalAgreementExpandedModel
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Projector = new GetProjectorModel
                    {
                        Id = x.Projector.Id,
                        Model = x.Projector.Model,
                        Brand = x.Projector.Brand,
                        SerialNumber = x.Projector.SerialNumber,
                        Damages = x.Projector.Damages,
                        Remarks = x.Projector.Remarks,
                        ExpositionId = x.Projector.ExpositionId,
                        Available = x.Projector.Available,
                        CreatedAt = x.Projector.CreatedAt,
                        UpdatedAt = x.Projector.UpdatedAt,
                    },
                    Place = new GetPlaceReducedModel
                    {
                        Id = x.Place.Id,
                        Name = x.Place.Name,
                        Description = x.Place.Description,
                        Address = x.Place.Address,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.CreatedAt,
                    },
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                }).AsNoTracking()
            .ToListAsync();

            return rentalAgreements;
        }

        public async Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreementsMineAvailable(Guid expositionId)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Forbidden to access this page");
            }
            Guid userId = new Guid(_user.Identity.Name);

            List<GetRentalAgreementExpandedModel> rentalAgreements = await _context.RentalAgreements
                .Where(ra => ra.Place.ExhibitorPlaces.Any(ep => ep.ExhibitorId == userId) &&
                             (ra.Projector.ExpositionId == null || ra.Projector.ExpositionId == expositionId))
                .Select(ra => new GetRentalAgreementExpandedModel
                {
                    Id = ra.Id,
                    StartDate = ra.StartDate,
                    EndDate = ra.EndDate,
                    Projector = new GetProjectorModel
                    {
                        Id = ra.Projector.Id,
                        Model = ra.Projector.Model,
                        Brand = ra.Projector.Brand,
                        SerialNumber = ra.Projector.SerialNumber,
                        Damages = ra.Projector.Damages,
                        Remarks = ra.Projector.Remarks,
                        ExpositionId = ra.Projector.ExpositionId,
                        Available = ra.Projector.Available,
                        CreatedAt = ra.Projector.CreatedAt,
                        UpdatedAt = ra.Projector.UpdatedAt,
                    },
                    Place = new GetPlaceReducedModel
                    {
                        Id = ra.Place.Id,
                        Name = ra.Place.Name,
                        Description = ra.Place.Description,
                        Address = ra.Place.Address,
                        CreatedAt = ra.CreatedAt,
                        UpdatedAt = ra.CreatedAt,
                    },
                    CreatedAt = ra.CreatedAt,
                    UpdatedAt = ra.UpdatedAt,
                })
                .AsNoTracking()
                .ToListAsync();

            return rentalAgreements;
        }

        public async Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreementsMineAvailable()
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Forbidden to access this page");
            }
            Guid userId = new Guid(_user.Identity.Name);

            List<GetRentalAgreementExpandedModel> rentalAgreements = await _context.RentalAgreements
                .Where(ra => ra.Place.ExhibitorPlaces.Any(ep => ep.ExhibitorId == userId) &&
                             ra.Projector.ExpositionId == null)
                .Select(ra => new GetRentalAgreementExpandedModel
                {
                    Id = ra.Id,
                    StartDate = ra.StartDate,
                    EndDate = ra.EndDate,
                    Projector = new GetProjectorModel
                    {
                        Id = ra.Projector.Id,
                        Model = ra.Projector.Model,
                        Brand = ra.Projector.Brand,
                        SerialNumber = ra.Projector.SerialNumber,
                        Damages = ra.Projector.Damages,
                        Remarks = ra.Projector.Remarks,
                        ExpositionId = ra.Projector.ExpositionId,
                        Available = ra.Projector.Available,
                        CreatedAt = ra.Projector.CreatedAt,
                        UpdatedAt = ra.Projector.UpdatedAt,
                    },
                    Place = new GetPlaceReducedModel
                    {
                        Id = ra.Place.Id,
                        Name = ra.Place.Name,
                        Description = ra.Place.Description,
                        Address = ra.Place.Address,
                        CreatedAt = ra.CreatedAt,
                        UpdatedAt = ra.CreatedAt,
                    },
                    CreatedAt = ra.CreatedAt,
                    UpdatedAt = ra.UpdatedAt,
                })
                .AsNoTracking()
                .ToListAsync();

            return rentalAgreements;
        }


        public async Task<GetRentalAgreementModel> PostRentalAgreement(PostRentalAgreementModel postRentalAgreementModel)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Forbidden to access this page");
            }
            var rentalAgreement = new RentalAgreement
            {
                PlaceId = postRentalAgreementModel.PlaceId,
                ProjectorId = postRentalAgreementModel.ProjectorId,
                StartDate = postRentalAgreementModel.StartDate,
                EndDate = postRentalAgreementModel.EndDate,
            };

            _context.RentalAgreements.Add(rentalAgreement);
            await _context.SaveChangesAsync();

            var getRentalAgreementModel = new GetRentalAgreementModel
                {
                    Id = rentalAgreement.Id,
                    ProjectorId = rentalAgreement.ProjectorId,
                    StartDate = rentalAgreement.StartDate,
                    EndDate = rentalAgreement.EndDate,
                    CreatedAt = rentalAgreement.CreatedAt,
                    UpdatedAt = rentalAgreement.UpdatedAt,
                };
            return getRentalAgreementModel;
        }

        public async Task<GetRentalAgreementModel> PutRentalAgreement(Guid id, PutRentalAgreementModel putRentalAgreementModel)
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Forbidden to access this page");
            }

            var rentalAgreement = await _context.RentalAgreements
            .Include(ra => ra.Projector)
            .SingleOrDefaultAsync(ra => ra.Id == id);

            if (rentalAgreement == null)
            {
                throw new NotFoundException("Rental Agreement not found.");
            }

            if (rentalAgreement.ProjectorId != putRentalAgreementModel.ProjectorId)
            {
                if (rentalAgreement.Projector != null && rentalAgreement.Projector.ExpositionId.HasValue)
                {
                    var expositionId = rentalAgreement.Projector.ExpositionId.Value;

                    // Remove exposition categories and artpieces
                    var expositionCategories = _context.ExpositionCategories.Where(ec => ec.ExpositionId == expositionId);
                    var expositionArtpieces = _context.ExpositionArtpieces.Where(ea => ea.ExpositionId == expositionId);
                    _context.ExpositionCategories.RemoveRange(expositionCategories);
                    _context.ExpositionArtpieces.RemoveRange(expositionArtpieces);

                    rentalAgreement.Projector.ExpositionId = null;
                    _context.Projectors.Update(rentalAgreement.Projector);
                }

                // Update the rental agreement with the new details
                rentalAgreement.ProjectorId = putRentalAgreementModel.ProjectorId;
            }

            rentalAgreement.PlaceId = putRentalAgreementModel.PlaceId;
            rentalAgreement.StartDate = putRentalAgreementModel.StartDate;
            rentalAgreement.EndDate = putRentalAgreementModel.EndDate;

            _context.RentalAgreements.Update(rentalAgreement);
            await _context.SaveChangesAsync();

            var getRentalAgreementModel = new GetRentalAgreementModel
            {
                Id = rentalAgreement.Id,
                ProjectorId = rentalAgreement.ProjectorId,
                StartDate = rentalAgreement.StartDate,
                EndDate = rentalAgreement.EndDate,
                CreatedAt = rentalAgreement.CreatedAt,
                UpdatedAt = rentalAgreement.UpdatedAt,
            };
            return getRentalAgreementModel;
        }

    }
}
