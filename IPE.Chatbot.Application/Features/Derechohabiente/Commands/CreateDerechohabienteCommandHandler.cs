using IPE.Chatbot.Application.Features.Derechohabiente.DTOs;
using IPE.Chatbot.Domain.Entities.chatBot;
using IPE.Chatbot.Persistence;
using MediatR;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Commands
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
            var derechohabiente = new DerechohabienteEntity
            {
                Nombre = request.Nombre,
                Clave = request.Clave
            };

            _context.Derechohabientes.Add(derechohabiente);
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
