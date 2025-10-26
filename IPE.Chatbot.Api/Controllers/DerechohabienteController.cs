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
    public class DerechohabienteController : ControllerBase
    {
        private readonly ILogger<DerechohabienteController> _logger;
        private readonly IMediator _mediator;

        public DerechohabienteController(ILogger<DerechohabienteController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DerechohabienteDto>>> GetAll()
        {
            _logger.LogInformation("GetAll Derechohabientes endpoint called.");
            var result = await _mediator.Send(new GetAllDerechohabientesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DerechohabienteDto>> GetById(int id)
        {
            _logger.LogInformation("GetById Derechohabiente endpoint called with id: {Id}", id);
            var result = await _mediator.Send(new GetDerechohabienteByIdQuery { Id = id });
            
            if (result == null)
            {
                return NotFound(new { message = $"Derechohabiente with Id {id} not found." });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DerechohabienteDto>> Create([FromBody] CreateDerechohabienteDto dto)
        {
            _logger.LogInformation("Create Derechohabiente endpoint called.");
            var command = new CreateDerechohabienteCommand
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Tipo = dto.Tipo,
                Flujo = dto.Flujo,
                Paso = dto.Paso,
                Folio = dto.Folio
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DerechohabienteDto>> Update(int id, [FromBody] UpdateDerechohabienteDto dto)
        {
            _logger.LogInformation("Update Derechohabiente endpoint called with id: {Id}", id);
            
            if (id != dto.Id)
            {
                return BadRequest(new { message = "Id in URL does not match Id in request body." });
            }

            try
            {
                var command = new UpdateDerechohabienteCommand
                {
                    Id = dto.Id,
                    Nombre = dto.Nombre,
                    Telefono = dto.Telefono,
                    Tipo = dto.Tipo,
                    Flujo = dto.Flujo,
                    Paso = dto.Paso,
                    Folio = dto.Folio
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete Derechohabiente endpoint called with id: {Id}", id);
            var result = await _mediator.Send(new DeleteDerechohabienteCommand { Id = id });
            
            if (!result)
            {
                return NotFound(new { message = $"Derechohabiente with Id {id} not found." });
            }

            return NoContent();
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint called.");
            return Ok("DerechohabienteController is working!");
        }

        [HttpPost("update-state")]
        public async Task<ActionResult> UpdateState([FromBody] UpdateStateDto dto)
        {
            _logger.LogInformation("UpdateState endpoint called for phone: {Telefono}", dto.Telefono);
            
            if (string.IsNullOrEmpty(dto.Telefono))
            {
                return BadRequest(new { message = "Teléfono requerido" });
            }

            var command = new UpdateStateCommand
            {
                Telefono = dto.Telefono,
                Flujo = dto.Flujo,
                Paso = dto.Paso
            };

            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return BadRequest(new { success = false, message = "Failed to update state" });
            }

            return Ok(new { success = true });
        }

        [HttpGet("get-state/{telefono}")]
        public async Task<ActionResult<StateDto>> GetState(string telefono)
        {
            _logger.LogInformation("GetState endpoint called for phone: {Telefono}", telefono);
            
            if (string.IsNullOrEmpty(telefono))
            {
                return BadRequest(new { message = "Teléfono requerido" });
            }

            var query = new GetStateByPhoneQuery { Telefono = telefono };
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { message = $"State for phone {telefono} not found." });
            }

            return Ok(result);
        }
    }
}
