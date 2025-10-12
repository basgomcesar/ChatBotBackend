using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Commands
{
    public class UpdateDerechohabienteCommand : IRequest<DerechohabienteDto?>
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }
}
