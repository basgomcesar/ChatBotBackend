using IPE.Chatbot.Application.Features.Dashboard.DTOs;
using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Persistence;
using MediatR;

namespace IPE.Chatbot.Application.Features.Dashboard.Queries
{
    public class GetDashboardSimulacionHandler : IRequestHandler<GetDashboardSimulacionQuery, DashboardSimulacionDto>
    {
        private readonly ChatbotDbContext _context;
        public GetDashboardSimulacionHandler(ChatbotDbContext context)
        {
            _context = context;
        }
        public Task<DashboardSimulacionDto> Handle(GetDashboardSimulacionQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            //Seleccionar el total de simulaciones realizadas 
            var solicitationsToday = _context.SolicitudesSimulacion.Where(s => s.FechaSolicitud >= today && s.FechaSolicitud < tomorrow);
            var totalSolicitationsToday = solicitationsToday.Count();
            var dashboardDto = new DashboardSimulacionDto
            {
                TotalSolicitationsToday = totalSolicitationsToday,
                SolicitudesSimulacion = solicitationsToday.Select(s => new SolicitudesSimulacionDto
                {
                    Id = s.Id,
                    NombreDerechohabiente = s.Derechohabiente.Nombre,
                    Telefono = s.Derechohabiente.Telefono,
                    TipoSimulacion = s.TipoSimulacion,
                    FechaSolicitud = s.FechaSolicitud,
                    Estado = s.Estado
                }).ToList()
            };
            return Task.FromResult(dashboardDto);
        }
    }
}
