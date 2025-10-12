using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class UpdateDerechohabienteCommand : IRequest<DerechohabienteDto>
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public string Flujo { get; set; } = string.Empty;
        public string Paso { get; set; } = string.Empty;
        public string Folio { get; set; } = string.Empty;
    }
}
