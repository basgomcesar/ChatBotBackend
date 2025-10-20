using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
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
            var entity = await _context.Derechohabientes
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                return false;
            }

            _context.Derechohabientes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
