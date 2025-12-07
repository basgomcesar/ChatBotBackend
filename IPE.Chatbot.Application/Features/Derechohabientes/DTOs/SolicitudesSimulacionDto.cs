namespace IPE.Chatbot.Application.Features.Derechohabientes.DTOs
{
    public class SolicitudesSimulacionDto
    {
        public int Id { get; set; }
        public string NombreDerechohabiente { get; set; } = string.Empty;
        public string Telefono { get; set; }
        public string? TipoSimulacion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
