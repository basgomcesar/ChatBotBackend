using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Queries
{
    public class GetAllDerechohabientesQuery : IRequest<List<DerechohabienteDto>>
    {
    }
}
