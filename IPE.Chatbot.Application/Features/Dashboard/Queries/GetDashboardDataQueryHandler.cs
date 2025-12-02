using IPE.Chatbot.Application.Features.Dashboard.DTOs;
using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Dashboard.Queries
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDto>
    {
        private readonly ChatbotDbContext _context;

        public GetDashboardDataQueryHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Get all derechohabientes who have UltimaInteraccion today
            var derechohabientesHoy = await _context.Derechohabientes
                .Where(d => d.UltimaInteraccion.HasValue && 
                           d.UltimaInteraccion.Value >= today && 
                           d.UltimaInteraccion.Value < tomorrow)
                .ToListAsync(cancellationToken);

            var derechohabientesDto = derechohabientesHoy.Select(entity => new DerechohabienteDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre ?? string.Empty,
                Telefono = entity.Telefono ?? string.Empty,
                Tipo = entity.Tipo ?? string.Empty,
                Flujo = entity.Flujo ?? string.Empty,
                Paso = entity.Paso ?? string.Empty,
                Folio = entity.Folio ?? string.Empty
            }).ToList();

            return new DashboardDto
            {
                TotalUsersToday = derechohabientesDto.Count,
                Derechohabientes = derechohabientesDto
            };
        }
    }
}
