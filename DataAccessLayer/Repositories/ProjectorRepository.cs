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

        public ProjectorRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetProjectorModel> GetProjector(Guid id)
        {
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
                throw new NotFoundException("Place Not Found");
            }
            return projector;
        }

        public async Task<List<GetProjectorModel>> GetProjectors()
        {
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

        public async Task<GetProjectorModel> PutProjector(Guid id, PutProjectorModel putProjectorModel)
        {
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

        public async Task<GetProjectorModel> PatchProjector(Guid id, PatchProjectorModel patchProjectorModel)
        {
            var projector = await _context.Projectors.FindAsync(id);
            if (projector == null)
            {
                throw new NotFoundException("Projector Not Found");
            }

            projector.ExpositionId = patchProjectorModel.ExpositionId ?? projector.ExpositionId;
            projector.Brand = patchProjectorModel.Brand ?? projector.Brand;
            projector.Model = patchProjectorModel.Model ?? projector.Model;
            projector.SerialNumber = patchProjectorModel.SerialNumber ?? projector.SerialNumber;
            projector.Damages = patchProjectorModel.Damages ?? projector.Damages;
            projector.Remarks = patchProjectorModel.Remarks ?? projector.Remarks;
            projector.Available = patchProjectorModel.Available ?? projector.Available;

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
