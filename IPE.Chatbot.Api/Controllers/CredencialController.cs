using IPE.Chatbot.Application.Features.Credencial.DTOs;
using IPE.Chatbot.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPE.Chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredencialController : ControllerBase
    {
        private readonly ILogger<CredencialController> _logger;
        private readonly IOcrService _ocrService;
        private readonly IWebHostEnvironment _environment;

        public CredencialController(
            ILogger<CredencialController> logger,
            IOcrService ocrService,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _ocrService = ocrService;
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<CredencialOcrResultDto>> UploadCredencial(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded or file is empty" });
            }

            // Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid file type. Allowed types: jpg, jpeg, png, bmp, tiff, gif" });
            }

            try
            {
                // Create directory if it doesn't exist
                var credencialesPath = Path.Combine(_environment.ContentRootPath, "media", "credenciales");
                if (!Directory.Exists(credencialesPath))
                {
                    Directory.CreateDirectory(credencialesPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = Path.Combine(credencialesPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File saved: {FileName}", fileName);

                // Process OCR
                var result = await _ocrService.ExtractCredencialDataAsync(filePath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing credencial image");
                return StatusCode(500, new { message = "Error processing the image" });
            }
        }
    }
}
