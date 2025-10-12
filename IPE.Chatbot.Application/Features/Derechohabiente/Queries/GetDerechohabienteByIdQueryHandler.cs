using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Queries
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
            var derechohabiente = await _context.Derechohabientes
                .Where(d => d.Id == request.Id)
                .Select(d => new DerechohabienteDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Clave = d.Clave
                })
                .FirstOrDefaultAsync(cancellationToken);

            return derechohabiente;
        }
    }
}
