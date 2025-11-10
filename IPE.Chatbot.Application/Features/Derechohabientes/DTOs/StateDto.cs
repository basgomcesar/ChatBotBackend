namespace IPE.Chatbot.Application.Features.Derechohabientes.DTOs
{
    public class StateDto
    {
        public string Telefono { get; set; } = string.Empty;
        public string Flujo { get; set; } = string.Empty;
        public string Paso { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Folio { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime? UltimaInteraccion { get; set; }
    }
}
