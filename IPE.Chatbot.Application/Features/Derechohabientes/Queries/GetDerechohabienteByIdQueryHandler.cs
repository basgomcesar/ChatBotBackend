using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Queries
{
    public class GetDerechohabienteByIdQueryHandler : IRequestHandler<GetDerechohabienteByIdQuery, DerechohabienteDto?>
    {
        private readonly ChatbotDbContext _context;

        public GetDerechohabienteByIdQueryHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<DerechohabienteDto?> Handle(GetDerechohabienteByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Derechohabientes
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                return null;
            }

            return new DerechohabienteDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Tipo = entity.Tipo,
                Flujo = entity.Flujo,
                Paso = entity.Paso,
                Folio = entity.Folio
            };
        }
    }
}
