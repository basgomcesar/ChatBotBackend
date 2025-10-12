using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Commands
{
    public class DeleteDerechohabienteCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
