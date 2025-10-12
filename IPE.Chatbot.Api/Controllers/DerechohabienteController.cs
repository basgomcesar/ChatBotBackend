using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPE.Chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DerechohabienteController : ControllerBase
    {
        private readonly ILogger<DerechohabienteController> _logger;
        public DerechohabienteController(ILogger<DerechohabienteController> logger)
        {
            _logger = logger;
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint called.");
            return Ok("DerechohabienteController is working!");
        }
    }
}
