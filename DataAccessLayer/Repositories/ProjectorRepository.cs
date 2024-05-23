using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Places;
using Models.Projectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProjectorRepository : IProjectorRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        //toDo fix forbiddenexceptions

        public ProjectorRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetProjectorModel> GetProjector(Guid id)
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            var projector = await _context.Projectors
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetProjectorModel
                {
                    Id = x.Id,
                    ExpositionId = x.ExpositionId,
                    Brand = x.Brand,
                    Model = x.Model,
                    SerialNumber = x.SerialNumber,
                    Damages = x.Damages,
                    Remarks = x.Remarks,
                    Available = x.Available,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .FirstOrDefaultAsync();
            if (projector == null)
            {
                throw new NotFoundException("Projector Not Found");
            }
            return projector;
        }

        public async Task<List<GetProjectorModel>> GetProjectors()
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var projectors = await _context.Projectors.Select(x => new GetProjectorModel
            {
                Id = x.Id,
                ExpositionId = x.ExpositionId,
                Brand = x.Brand,
                Model = x.Model,
                SerialNumber = x.SerialNumber,
                Damages = x.Damages,
                Remarks = x.Remarks,
                Available = x.Available,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).AsNoTracking()
            .ToListAsync();

            return projectors;
        }

        public async Task<GetProjectorModel> PostProjector(PostProjectorModel postProjectorModel)
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            var projector = new Projector
            {
                Brand = postProjectorModel.Brand,
                Model = postProjectorModel.Model,
                SerialNumber = postProjectorModel.SerialNumber,
                Damages = postProjectorModel.Damages,
                Remarks = postProjectorModel.Remarks,
                Available = true,
            };

            _context.Projectors.Add(projector);
            await _context.SaveChangesAsync();

            var getProjectorModel = new GetProjectorModel
            {
                Id = projector.Id,
                Brand = projector.Brand,
                Model = projector.Model,
                SerialNumber = projector.SerialNumber,
                Damages = projector.Damages,
                Remarks = projector.Remarks,
                Available = projector.Available,
                CreatedAt = projector.CreatedAt,
                UpdatedAt = projector.UpdatedAt,
            };
            return getProjectorModel;
        }

        public async Task<GetProjectorModel> PutProjector(Guid id, PutProjectorModel putProjectorModel)
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            var projector = await _context.Projectors.FindAsync(id);
            if (projector == null)
            {
                throw new NotFoundException("Projector Not Found");
            }

            projector.Brand = putProjectorModel.Brand;
            projector.Model = putProjectorModel.Model;
            projector.SerialNumber = putProjectorModel.SerialNumber;
            projector.Damages = putProjectorModel.Damages;
            projector.Remarks = putProjectorModel.Remarks;

            _context.Projectors.Update(projector);
            await _context.SaveChangesAsync();

            var getProjectorModel = new GetProjectorModel
            {
                Id = projector.Id,
                ExpositionId = projector.ExpositionId,
                Brand = projector.Brand,
                Model = projector.Model,
                SerialNumber = projector.SerialNumber,
                Damages = projector.Damages,
                Remarks = projector.Remarks,
                Available = projector.Available,
                CreatedAt = projector.CreatedAt,
                UpdatedAt = projector.UpdatedAt,
            };
            return getProjectorModel;
        }

        public async Task<List<GetProjectorModel>> GetAvailableProjectorsByDates(DateTime startDate, DateTime endDate)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            var unavailableProjectorIds = _context.RentalAgreements
            .Where(res => res.StartDate < endDate && res.EndDate > startDate)
            .Select(res => res.ProjectorId)
            .Distinct();

            var availableProjectors = await _context.Projectors
            .Where(p => !unavailableProjectorIds.Contains(p.Id) && p.Available) // Filters out projectors that are unavailable and checks the Available flag
            .Select(x => new GetProjectorModel
            {
                Id = x.Id,
                ExpositionId = x.ExpositionId,
                Brand = x.Brand,
                Model = x.Model,
                SerialNumber = x.SerialNumber,
                Damages = x.Damages,
                Remarks = x.Remarks,
                Available = x.Available,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .AsNoTracking()
            .ToListAsync();

            return availableProjectors;
        }

        public async Task<List<Guid>> GetReservedProjectors()
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var unavailableProjectorIds = await _context.RentalAgreements
            .Where(res => res.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= res.EndDate)
            .Select(res => res.ProjectorId)
            .Distinct()
            .ToListAsync();

            return unavailableProjectorIds;
        }

        public async Task<List<GetProjectorModel>> GetAvailableProjectorsByDatesWithCurrent(Guid rentalagreementId, DateTime startDate, DateTime endDate)
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var currentRentalAgreement = await _context.RentalAgreements
            .Include(ra => ra.Projector)
            .FirstOrDefaultAsync(ra => ra.Id == rentalagreementId);

            if (currentRentalAgreement == null)
            {
                throw new NotFoundException("Rental Agreement not found.");
            }

            List<Guid> unavailableProjectorIds = _context.RentalAgreements
            .Where(res => res.StartDate < endDate && res.EndDate > startDate)
            .Select(res => res.ProjectorId)
            .Distinct()
            .ToList();

            if (currentRentalAgreement.StartDate < endDate && currentRentalAgreement.EndDate > startDate)
            {
                unavailableProjectorIds.Remove(currentRentalAgreement.ProjectorId);
            }

            var availableProjectors = await _context.Projectors
            .Where(p => !unavailableProjectorIds.Contains(p.Id) && p.Available) // Filters out projectors that are unavailable and checks the Available flag
            .Select(x => new GetProjectorModel
            {
                Id = x.Id,
                ExpositionId = x.ExpositionId,
                Brand = x.Brand,
                Model = x.Model,
                SerialNumber = x.SerialNumber,
                Damages = x.Damages,
                Remarks = x.Remarks,
                Available = x.Available,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .AsNoTracking()
            .ToListAsync();

            return availableProjectors;
        }

        public async Task<GetProjectorModel> PatchProjector(Guid id, PatchProjectorModel patchProjectorModel)
        {
            var currentDateTime = DateTime.UtcNow;

            var unavailableProjectorIds = await _context.RentalAgreements
            .Where(res => res.StartDate <= currentDateTime && currentDateTime <= res.EndDate)
            .Select(res => res.ProjectorId)
            .Distinct()
            .ToListAsync();

            if (unavailableProjectorIds.Contains(id))
            {
                throw new ForbiddenException("Projector is currently reserved and cannot be updated.");
            }

            var projector = await _context.Projectors.FindAsync(id);
            if (projector == null)
            {
                throw new NotFoundException("Projector Not Found");
            }

            projector.Available = patchProjectorModel.Available;

            _context.Projectors.Update(projector);
            await _context.SaveChangesAsync();

            var getProjectorModel = new GetProjectorModel
            {
                Id = projector.Id,
                ExpositionId = projector.ExpositionId,
                Brand = projector.Brand,
                Model = projector.Model,
                SerialNumber = projector.SerialNumber,
                Damages = projector.Damages,
                Remarks = projector.Remarks,
                Available = projector.Available,
                CreatedAt = projector.CreatedAt,
                UpdatedAt = projector.UpdatedAt,
            };
            return getProjectorModel;
        }

    }
}
