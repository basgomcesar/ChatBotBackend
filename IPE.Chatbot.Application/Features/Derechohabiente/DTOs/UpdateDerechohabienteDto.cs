namespace IPE.Chatbot.Application.Features.Derechohabiente.DTOs
{
    public class UpdateDerechohabienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }
}
