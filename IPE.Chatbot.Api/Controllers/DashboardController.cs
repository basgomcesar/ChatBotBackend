using IPE.Chatbot.Application.Features.Dashboard.DTOs;
using IPE.Chatbot.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IPE.Chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard-data")]
        public async Task<ActionResult<DashboardDto>> GetDashboardData()
        {
            var result = await _mediator.Send(new GetDashboardDataQuery());
            return Ok(result);
        }
        [HttpGet("dashboard-simulacion")]
        public async Task<ActionResult<DashboardSimulacionDto>> GetDashboardSimualcion()
        {
            var result = await _mediator.Send(new GetDashboardSimulacionQuery());
            return Ok(result);
        }
        [HttpGet("dashboard-asesor")]
        public async Task<ActionResult<DashboardAsesorDto>> GetDashboardAsesor()
        {
            var result = await _mediator.Send(new GetDashboardAsesorQuery());
            return Ok(result);
        }
    }
}
