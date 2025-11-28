using IPE.Chatbot.Application.Interfaces;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IPE.Chatbot.Application.Features.Derechohabientes.Commands
{
    public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, bool>
    {
        private readonly ChatbotDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IChatbotNotificationService _notificationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateStateCommandHandler(
            ChatbotDbContext context,
            IDistributedCache cache,
            IChatbotNotificationService notificationService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _cache = cache;
            _notificationService = notificationService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<bool> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Telefono))
            {
                return false;
            }

            var cacheKey = $"chatbot:{request.Telefono}";

            // Intentar leer valor actual del cache
            var existing = await _cache.GetStringAsync(cacheKey, cancellationToken);

            JsonObject node;
            if (!string.IsNullOrEmpty(existing))
            {
                // Si existe, parsearlo a un JsonObject mutable
                var parsed = JsonNode.Parse(existing);
                node = parsed?.AsObject() ?? new JsonObject();
            }
            else
            {
                node = new JsonObject();
            }

            // Asegurar Telefono y actualizar UltimaInteraccion
            node["Telefono"] = request.Telefono;
            node["UltimaInteraccion"] = DateTime.UtcNow.ToString("o");

            // Solo sobreescribir campos que llegaron (no borrar los que no vienen)
            if (!string.IsNullOrEmpty(request.Flujo))
                node["Flujo"] = request.Flujo;

            if (!string.IsNullOrEmpty(request.Paso))
                node["Paso"] = request.Paso;

            if (!string.IsNullOrEmpty(request.Nombre))
                node["Nombre"] = request.Nombre;

            if (!string.IsNullOrEmpty(request.Folio))
                node["Folio"] = request.Folio;

            if (!string.IsNullOrEmpty(request.Tipo))
                node["Tipo"] = request.Tipo;

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            // Guardar el JSON fusionado
            await _cache.SetStringAsync(cacheKey, node.ToJsonString(), cacheOptions, cancellationToken);

            // Actualizar base de datos en scope separado (igual que antes), 
            // pero también solo con campos que llegaron para no sobreescribir con vacío.
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var scopedContext = scope.ServiceProvider.GetRequiredService<ChatbotDbContext>();

                    var user = await scopedContext.Derechohabientes
                        .FirstOrDefaultAsync(u => u.Telefono == request.Telefono);

                    if (user != null)
                    {
                        bool flujoCambio = false;
                        if (!string.IsNullOrEmpty(request.Flujo))
                            flujoCambio = user.Flujo != request.Flujo;
                            user.Flujo = request.Flujo;

                        if (!string.IsNullOrEmpty(request.Paso))
                            user.Paso = request.Paso;

                        if (!string.IsNullOrEmpty(request.Nombre))
                            user.Nombre = request.Nombre;

                        if (!string.IsNullOrEmpty(request.Folio))
                            user.Folio = request.Folio;

                        if (!string.IsNullOrEmpty(request.Tipo))
                            user.Tipo = request.Tipo;

                        user.UltimaInteraccion = DateTime.UtcNow;

                        if (flujoCambio)
                        {
                            switch (request.Flujo)
                            {
                                case "SIMULACION":
                                    scopedContext.SolicitudesSimulacion.Add(new Domain.Entities.chatBot.SolicitudesSimulacionEntity
                                        {
                                            DerechohabienteId = user.Id,
                                            TipoSimulacion = user.Tipo ?? "GENERAL",
                                            FechaSolicitud = DateTime.UtcNow,
                                            Estado = user.Paso ?? "SIMULACION_INICIAL"
                                    }
                                    );
                                    break;
                                case "ASESOR":
                                    user.Paso = "EsperandoAgente";
                                    break;
                                    // Agregar más casos según sea necesario
                            }
                        }

                        await scopedContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating database: {ex.Message}");
                }
            });

            // Notificar a clientes
            await _notificationService.NotifyStateUpdate(
                request.Telefono,
                request.Flujo,
                request.Paso);

            return true;
        }
    }
}