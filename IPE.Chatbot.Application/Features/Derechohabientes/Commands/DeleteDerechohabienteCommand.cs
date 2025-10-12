using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class DeleteDerechohabienteCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
