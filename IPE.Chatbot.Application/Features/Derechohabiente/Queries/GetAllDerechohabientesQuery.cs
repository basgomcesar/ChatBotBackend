using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Queries
{
    public class GetAllDerechohabientesQuery : IRequest<List<DerechohabienteDto>>
    {
    }
}
