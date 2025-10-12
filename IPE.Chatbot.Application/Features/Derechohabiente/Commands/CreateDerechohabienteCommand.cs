using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Commands
{
    public class CreateDerechohabienteCommand : IRequest<DerechohabienteDto>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }
}
