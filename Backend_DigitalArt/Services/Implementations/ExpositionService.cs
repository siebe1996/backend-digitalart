using Backend_DigitalArt.Hubs;
using DataAccessLayer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Models.Artpieces;
using Models.Expositions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend_DigitalAr.Services.Implementations
{

    public class ExpositionService
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHubContext<ExpositionHub> _hubContext;

        public ExpositionService(Backend_DigitalArtContext context, IHubContext<ExpositionHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task UpdateExpositionStatus(Guid expositionId, PatchExpositionModel patchExpositionModel)
        {
            var exposition = await _context.Expositions.Include(e => e.ExpositionArtpieces)
                                                       .ThenInclude(ea => ea.Artpiece)
                                                       .AsNoTracking()
                                                       .SingleOrDefaultAsync(e => e.Id == expositionId);

            if (exposition != null)
            {

                var artpieceTitles = exposition.ExpositionArtpieces.Select(ea => new GetArtpieceModel
                {
                    Id = ea.Artpiece.Id,
                    Title = ea.Artpiece.Title,
                    Description = ea.Artpiece.Description,
                    ImageData = ea.Artpiece.ImageData,
                    MimeTypeImageData = ea.Artpiece.MimeTypeImageData,
                    CreatedAt = ea.Artpiece.CreatedAt,
                    UpdatedAt = ea.Artpiece.UpdatedAt
                }).Distinct()
                    .ToList();

                if (patchExpositionModel.Active)
                {
                    await _hubContext.Clients.Group(expositionId.ToString()).SendAsync("DisplayArtpieces", artpieceTitles);
                }
                else
                {
                    await _hubContext.Clients.Group(expositionId.ToString()).SendAsync("StopDisplay");
                }
            }
        }
    }


}
