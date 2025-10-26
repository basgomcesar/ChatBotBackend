using IPE.Chatbot.Api.Hubs;
using IPE.Chatbot.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace IPE.Chatbot.Api.Services
{
    public class ChatbotNotificationService : IChatbotNotificationService
    {
        private readonly IHubContext<ChatbotHub> _hubContext;

        public ChatbotNotificationService(IHubContext<ChatbotHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyStateUpdate(string telefono, string flujo, string paso)
        {
            await _hubContext.Clients.All.SendAsync("UserStateUpdated", new
            {
                telefono,
                flujo,
                paso
            });
        }
    }
}
