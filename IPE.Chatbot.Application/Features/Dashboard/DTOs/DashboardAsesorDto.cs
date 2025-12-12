using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
namespace IPE.Chatbot.Application.Features.Dashboard.DTOs
{
    public class DashboardAsesorDto
    {
        public int TotalSolicitudesHoy {  get; set; }
        public List<SolicitudesAsesorDto> SolicitudesAsesor { get; set; } = new List<SolicitudesAsesorDto>();

    }
}
