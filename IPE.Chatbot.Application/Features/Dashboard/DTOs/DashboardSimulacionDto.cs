using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;

namespace IPE.Chatbot.Application.Features.Dashboard.DTOs
{
    public class DashboardSimulacionDto
    {
        public int TotalSolicitationsToday { get; set; }
        public List<SolicitudesSimulacionDto> SolicitudesSimulacion { get; set; } = new List<SolicitudesSimulacionDto>();
    }
}
