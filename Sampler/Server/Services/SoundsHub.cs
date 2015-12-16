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
            // Call the broadcastChatMessage method to update all clients.
            Clients.Others.broadcastChatMessage(name, message);
        }
    }
}