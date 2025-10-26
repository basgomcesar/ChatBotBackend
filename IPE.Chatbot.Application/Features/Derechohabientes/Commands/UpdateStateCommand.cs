using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class UpdateStateCommand : IRequest<bool>
    {
        public string Telefono { get; set; } = string.Empty;
        public string Flujo { get; set; } = string.Empty;
        public string Paso { get; set; } = string.Empty;
    }
}
