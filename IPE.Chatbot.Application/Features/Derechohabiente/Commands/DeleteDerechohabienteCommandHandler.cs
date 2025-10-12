using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabiente.Commands
{
    public class DeleteDerechohabienteCommandHandler : IRequestHandler<DeleteDerechohabienteCommand, bool>
    {
        private readonly ChatbotDbContext _context;

        public DeleteDerechohabienteCommandHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDerechohabienteCommand request, CancellationToken cancellationToken)
        {
            var derechohabiente = await _context.Derechohabientes
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (derechohabiente == null)
            {
                return false;
            }

            _context.Derechohabientes.Remove(derechohabiente);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
