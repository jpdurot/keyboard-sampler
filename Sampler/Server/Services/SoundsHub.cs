using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Sampler.Server.Services
{
    public class SoundsHub : Hub
    {
        /// <summary>
        /// Server function to be called on a client chat
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void ChatSend(string name, string message)
        {
            // Store previous chat in memory
            var chatMessage = UserService.Current.AddChatMessage(name, message);

            // Call the broadcastChatMessage method to update all clients.
            Clients.Others.broadcastChatMessage(name, message, chatMessage.Time);
        }

        public override Task OnConnected()
        {
            UserService.Current.AddConnectedSoundsHubUser();
            Clients.All.syncSoundsHubUserCount(UserService.Current.GetConnectedSoundsHubUserCount());
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserService.Current.RemoveConnectedSoundsHubUser();
            Clients.All.syncSoundsHubUserCount(UserService.Current.GetConnectedSoundsHubUserCount());
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            UserService.Current.AddConnectedSoundsHubUser();
            Clients.All.syncSoundsHubUserCount(UserService.Current.GetConnectedSoundsHubUserCount());
            return base.OnReconnected();
        }
    }
}