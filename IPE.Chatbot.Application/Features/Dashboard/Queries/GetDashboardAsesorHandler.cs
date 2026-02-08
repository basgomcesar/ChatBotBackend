using IPE.Chatbot.Application.Features.Dashboard.DTOs;
using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IPE.Chatbot.Application.Features.Dashboard.Queries
{
    public class GetDashboardAsesorHandler : IRequestHandler<GetDashboardAsesorQuery, DashboardAsesorDto>
    {
        private readonly ChatbotDbContext _context;

        public GetDashboardAsesorHandler(ChatbotDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardAsesorDto> Handle(GetDashboardAsesorQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Obtener solicitudes del día
            var solicitudesHoy = await _context.SolicitudesAsesor
                .Where(s => s.FechaSolicitud >= today && s.FechaSolicitud < tomorrow)
                .Select(s => new
                {
                    s.Id,
                    s.DerechohabienteId,
                    s.FechaSolicitud,
                    s.Derechohabiente.Nombre,
                    Estado = s.Estado.ToString()
                })
                .ToListAsync(cancellationToken);

            var solicitudesDto = solicitudesHoy.Select(s => new SolicitudesAsesorDto
            {
                Id = s.Id,
                DerechohabienteId = s.DerechohabienteId,
                FechaSolicitude = s.FechaSolicitud.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                NombreDerechohabiente = s.Nombre,
                NumeroTelefono = _context.Derechohabientes
                    .Where(d => d.Id == s.DerechohabienteId)
                    .Select(d => d.Telefono)
                    .FirstOrDefault(),
                Estado = s.Estado
            }).ToList();

            return new DashboardAsesorDto
            {
                TotalSolicitudesHoy = solicitudesDto.Count,
                SolicitudesAsesor = solicitudesDto
            };
        }
    }
}
