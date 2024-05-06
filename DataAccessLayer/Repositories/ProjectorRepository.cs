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
    public class ProjectorRepository
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
    }
}
