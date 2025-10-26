using Microsoft.AspNetCore.SignalR;

namespace IPE.Chatbot.Api.Hubs
{
    public class ChatbotHub : Hub
    {
        public async Task SendStateUpdate(string telefono, string flujo, string paso)
        {
            await Clients.All.SendAsync("UserStateUpdated", new
            {
                telefono,
                flujo,
                paso
            });
        }
    }
}
