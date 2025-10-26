using IPE.Chatbot.Application.Interfaces;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, bool>
    {
        private readonly ChatbotDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IChatbotNotificationService _notificationService;

        public UpdateStateCommandHandler(
            ChatbotDbContext context,
            IDistributedCache cache,
            IChatbotNotificationService notificationService)
        {
            _context = context;
            _cache = cache;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Telefono))
            {
                return false;
            }

            // Save to Redis cache (fast temporary storage)
            var cacheKey = $"chatbot:{request.Telefono}";
            var cacheData = new
            {
                request.Telefono,
                request.Flujo,
                request.Paso,
                UltimaInteraccion = DateTime.UtcNow
            };
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(cacheData),
                cacheOptions,
                cancellationToken);

            // Update database asynchronously (non-blocking)
            _ = Task.Run(async () =>
            {
                try
                {
                    var user = await _context.Derechohabientes
                        .FirstOrDefaultAsync(u => u.Telefono == request.Telefono);

                    if (user != null)
                    {
                        user.Flujo = request.Flujo;
                        user.Paso = request.Paso;
                        user.UltimaInteraccion = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    // Log error if needed, but don't block the main request
                }
            }, cancellationToken);

            // Notify all connected clients via SignalR
            await _notificationService.NotifyStateUpdate(
                request.Telefono,
                request.Flujo,
                request.Paso);

            return true;
        }
    }
}
