using IPE.Chatbot.Application.Features.Derechohabientes.Commands;
using IPE.Chatbot.Application.Features.Derechohabientes.DTOs;
using IPE.Chatbot.Application.Features.Derechohabientes.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPE.Chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController: ControllerBase
    {
        [HttpGet(Name = "dashboard-data")]
        public string Get()
        {
            return "";
        }
    }
}
