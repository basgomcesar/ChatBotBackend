using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class UpdateDerechohabienteCommandHandler : IRequestHandler<UpdateDerechohabienteCommand, DerechohabienteDto>
    {
        private readonly ChatbotDbContext _context;

        public UpdateDerechohabienteCommandHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<DerechohabienteDto> Handle(UpdateDerechohabienteCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Derechohabientes
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Derechohabiente with Id {request.Id} not found.");
            }

            entity.Nombre = request.Nombre;
            entity.Telefono = request.Telefono;
            entity.Tipo = request.Tipo;
            entity.Flujo = request.Flujo;
            entity.Paso = request.Paso;
            entity.Folio = request.Folio;

            await _context.SaveChangesAsync(cancellationToken);

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
