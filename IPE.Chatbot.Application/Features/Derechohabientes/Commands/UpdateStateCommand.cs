using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class UpdateStateCommand : IRequest<bool>
    {
        public string Telefono { get; set; } = string.Empty;
        public string Flujo { get; set; } = string.Empty;
        public string Paso { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Folio { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string TipoPrestamo { get; set; } = string.Empty;
        public string NumeroAfiliacion { get; set; } = string.Empty;
        public int? NumeroDeAvalesProcesados { get; set; }
        public List<AvalDto>? Avales { get; set; }
    }
}
