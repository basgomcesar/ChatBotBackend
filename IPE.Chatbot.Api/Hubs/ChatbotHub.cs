using Microsoft.AspNetCore.SignalR;

namespace IPE.Chatbot.Api.Hubs
{
    public class ChatbotHub : Hub
    {
        // Hub for SignalR real-time communication
        // Client connections are managed automatically by SignalR
        // Notifications are sent via IChatbotNotificationService
    }
}
