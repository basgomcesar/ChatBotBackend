using IPE.Chatbot.Application.Features.Credencial.DTOs;

namespace IPE.Chatbot.Application.Interfaces
{
    public interface IOcrService
    {
        Task<CredencialOcrResultDto> ExtractCredencialDataAsync(string imagePath);
    }
}
