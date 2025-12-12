using Microsoft.AspNetCore.SignalR;

namespace IPE.Chatbot.Api.Hubs
{
    public class ChatbotHub : Hub
    {
        // Hub for SignalR real-time communication
        // Client connections are managed automatically by SignalR
        // Notifications are sent via IChatbotNotificationService
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
        public async Task JoinRoom(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            Console.WriteLine($"Cliente {Context.ConnectionId} unido a room {room}");
        }
        // Enviar mensaje desde el asesor backend room
        public async Task SendAdvisorMessage(string room, string message)
        {
            //Aqui llama a Baileys para enviar el mensaje a WhatsApp
            await Clients.Group(room).SendAsync("chatUpdate", new
            {
                fromAdvisor = true,
                text = message
            });
        }
        // Enviar mensaje desde el backend (cuando WhatsApp enviia algo)
        public async Task SendWhatsAppMessageToRoom(string room, object message)
        {
            await Clients.Group(room).SendAsync("chatUpdate", message);
        }
        public async Task AdvisorError(string room, string error)
        {
            await Clients.Group(room).SendAsync("advisorError", error);
        }
    }
}
