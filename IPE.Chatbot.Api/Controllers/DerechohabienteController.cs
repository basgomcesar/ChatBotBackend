using IPE.Chatbot.Application.Features.Derechohabiente.Commands;
using IPE.Chatbot.Application.Features.Derechohabiente.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IPE.Chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DerechohabienteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DerechohabienteController> _logger;

        public DerechohabienteController(IMediator mediator, ILogger<DerechohabienteController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDerechohabientesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetDerechohabienteByIdQuery { Id = id });
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDerechohabienteCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDerechohabienteCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _mediator.Send(command);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteDerechohabienteCommand { Id = id });
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
