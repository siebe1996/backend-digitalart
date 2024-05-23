using Microsoft.AspNetCore.SignalR;

namespace Backend_DigitalArt.Hubs
{
    public class ProjectorHub : Hub
    {
        public async Task JoinProjectorGroup(string projectorId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectorId);
        }

        public async Task LeaveProjectorGroup(string projectorId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectorId);
        }

        public Task SendUpdateToProjector(string projectorId, string message)
        {
            return Clients.Group(projectorId).SendAsync("ReceiveMessage", message);
        }
    }

}
