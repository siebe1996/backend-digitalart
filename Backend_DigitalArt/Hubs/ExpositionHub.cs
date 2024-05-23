using Microsoft.AspNetCore.SignalR;
using Models.Artpieces;

namespace Backend_DigitalArt.Hubs
{
    public class ExpositionHub : Hub
    {
        public async Task JoinExpositionGroup(string expositionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, expositionId);
        }

        public async Task LeaveExpositionGroup(string expositionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, expositionId);
        }
        public async Task StartExposition(string expositionId, List<GetArtpieceModel> artpieces)
        {
            await Clients.Group(expositionId).SendAsync("DisplayArtpieces", artpieces);
        }

        public async Task StopExposition(string expositionId)
        {
            await Clients.Group(expositionId).SendAsync("StopDisplay");
        }
    }
}
