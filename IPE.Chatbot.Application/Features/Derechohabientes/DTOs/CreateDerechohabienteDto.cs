namespace IPE.Chatbot.Application.Features.Derechohabientes.DTOs
{
    public class CreateDerechohabienteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public string Flujo { get; set; } = string.Empty;
        public string Paso { get; set; } = string.Empty;
        public string Folio { get; set; } = string.Empty;
    }
}
