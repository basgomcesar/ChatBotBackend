using IPE.Chatbot.Application.Features.Dashboard.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPE.Chatbot.Application.Features.Dashboard.Queries
{
    public class GetDashboardSimulacionQuery : IRequest<DashboardSimulacionDto>
    {
    }
}
