using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Queries
{
    public class GetAllDerechohabientesQueryHandler : IRequestHandler<GetAllDerechohabientesQuery, List<DerechohabienteDto>>
    {
        private readonly ChatbotDbContext _context;

        public GetAllDerechohabientesQueryHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<List<DerechohabienteDto>> Handle(GetAllDerechohabientesQuery request, CancellationToken cancellationToken)
        {
            var derechohabientes = await _context.Derechohabientes
                .Select(d => new DerechohabienteDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Clave = d.Clave
                })
                .ToListAsync(cancellationToken);

            return derechohabientes;
        }
    }
}
