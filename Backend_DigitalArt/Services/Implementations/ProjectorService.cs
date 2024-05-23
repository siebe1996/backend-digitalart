using Backend_DigitalArt.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Services.Implementations
{
    public class ProjectorService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProjectorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ChangeProjectorExpositionStatus(string projectorId, bool isActive)
        {
            // Pretend we update the database here
            // var projector = dbContext.Projectors.Find(projectorId);
            // projector.IsActive = isActive;
            // dbContext.SaveChanges();

            // Notify connected clients
            var hubContext = _serviceProvider.GetRequiredService<IHubContext<ProjectorHub>>();
            await hubContext.Clients.Group(projectorId).SendAsync("UpdateProjectorStatus", projectorId, isActive);
        }
    }

}
