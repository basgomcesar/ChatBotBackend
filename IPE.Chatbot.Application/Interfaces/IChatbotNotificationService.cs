namespace IPE.Chatbot.Application.Interfaces
{
    public interface IChatbotNotificationService
    {
        Task NotifyStateUpdate(string telefono, string flujo, string paso);
    }
}
