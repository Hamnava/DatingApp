using Business.PublicClasses;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            var currentUser = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisConnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            var currentUser = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);

            await base.OnDisconnectedAsync(exception);

        }
    }
}
