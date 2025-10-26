using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Queries
{
    public class GetStateByPhoneQueryHandler : IRequestHandler<GetStateByPhoneQuery, StateDto?>
    {
        private readonly ChatbotDbContext _context;
        private readonly IDistributedCache _cache;

        public GetStateByPhoneQueryHandler(ChatbotDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<StateDto?> Handle(GetStateByPhoneQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Telefono))
            {
                return null;
            }

            // Try to get from cache first (fast)
            var cacheKey = $"chatbot:{request.Telefono}";
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var stateDto = JsonSerializer.Deserialize<StateDto>(cachedData);
                    if (stateDto != null)
                    {
                        return stateDto;
                    }
                }
                catch (JsonException)
                {
                    // If deserialization fails, fall through to database query
                }
            }

            // If not in cache, get from database
            var entity = await _context.Derechohabientes
                .FirstOrDefaultAsync(d => d.Telefono == request.Telefono, cancellationToken);

            if (entity == null)
            {
                return null;
            }

            var result = new StateDto
            {
                Telefono = entity.Telefono,
                Flujo = entity.Flujo,
                Paso = entity.Paso,
                UltimaInteraccion = entity.UltimaInteraccion
            };

            // Cache the result for future requests
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                cacheOptions,
                cancellationToken);

            return result;
        }
    }
}
