using IPE.Chatbot.Application.Features.Dashboard.DTOs;
using MediatR;

namespace IPE.Chatbot.Application.Features.Dashboard.Queries
{
    public class GetDashboardDataQuery : IRequest<DashboardDto>
    {
    }
}
