using Microsoft.AspNet.SignalR;

namespace Sampler.Server.Services
{
    public class SoundsHub : Hub 
    {
        public void BroadcastSoundPlayed(string sound, string user)
        {
            Clients.All.addNewMessageToPage(sound, user);
        }
    }
}