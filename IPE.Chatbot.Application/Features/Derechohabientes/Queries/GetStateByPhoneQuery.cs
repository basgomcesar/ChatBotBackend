using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Queries
{
    public class GetStateByPhoneQuery : IRequest<StateDto?>
    {
        public string Telefono { get; set; } = string.Empty;
    }
}
