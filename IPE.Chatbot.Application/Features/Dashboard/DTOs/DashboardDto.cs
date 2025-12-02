using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;

namespace IPE.Chatbot.Application.Features.Dashboard.DTOs
{
    public class DashboardDto
    {
        public int TotalUsersToday { get; set; }
        public List<DerechohabienteDto> Derechohabientes { get; set; } = new List<DerechohabienteDto>();
    }
}
