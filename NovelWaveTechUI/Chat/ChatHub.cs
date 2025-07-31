using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace NovelWaveTechUI.Chat
{

    public class ChatHub : Hub
    {
        // SendMessage invoked by clients, broadcasts to all connected clients
        public async Task SendMessage(string user, string message)
        {
            var timestamp = System.DateTime.Now.ToString("HH:mm");
            await Clients.All.SendAsync("ReceiveMessage", user, message, timestamp);
        }
    }


}
