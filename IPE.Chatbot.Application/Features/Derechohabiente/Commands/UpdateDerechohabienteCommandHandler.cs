using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Commands
{
    public class UpdateDerechohabienteCommandHandler : IRequestHandler<UpdateDerechohabienteCommand, DerechohabienteDto?>
    {
        private readonly ChatbotDbContext _context;

        public UpdateDerechohabienteCommandHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<DerechohabienteDto?> Handle(UpdateDerechohabienteCommand request, CancellationToken cancellationToken)
        {
            var derechohabiente = await _context.Derechohabientes
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (derechohabiente == null)
            {
                return null;
            }

            derechohabiente.Nombre = request.Nombre;
            derechohabiente.Clave = request.Clave;

            await _context.SaveChangesAsync(cancellationToken);

            return new DerechohabienteDto
            {
                Id = derechohabiente.Id,
                Nombre = derechohabiente.Nombre,
                Clave = derechohabiente.Clave
            };
        }
    }
}
