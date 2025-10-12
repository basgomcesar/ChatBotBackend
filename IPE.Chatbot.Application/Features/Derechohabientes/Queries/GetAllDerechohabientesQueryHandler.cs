using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Queries
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
            var entities = await _context.Derechohabientes
                .ToListAsync(cancellationToken);

            return entities.Select(entity => new DerechohabienteDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Tipo = entity.Tipo,
                Flujo = entity.Flujo,
                Paso = entity.Paso,
                Folio = entity.Folio
            }).ToList();
        }
    }
}
