using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Domain.Entities.chatBot;
using IPE.Chatbot.Persistence;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class CreateDerechohabienteCommandHandler : IRequestHandler<CreateDerechohabienteCommand, DerechohabienteDto>
    {
        private readonly ChatbotDbContext _context;

        public CreateDerechohabienteCommandHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<DerechohabienteDto> Handle(CreateDerechohabienteCommand request, CancellationToken cancellationToken)
        {
            var entity = new DerechohabienteEntity
            {
                Nombre = request.Nombre,
                Telefono = request.Telefono,
                Tipo = request.Tipo,
                Flujo = request.Flujo,
                Paso = request.Paso,
                Folio = request.Folio
            };

            _context.Derechohabientes.Add(entity);
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
