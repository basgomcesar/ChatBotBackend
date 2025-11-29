using System.Text.RegularExpressions;
using IPE.Chatbot.Application.Features.Credencial.DTOs;
using IPE.Chatbot.Application.Interfaces;
using Tesseract;

namespace IPE.Chatbot.Api.Services
{
    public class OcrService : IOcrService
    {
        private readonly ILogger<OcrService> _logger;
        private readonly string _tessDataPath;

        public OcrService(ILogger<OcrService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _tessDataPath = configuration["TesseractDataPath"] ?? "./tessdata";
        }

        public async Task<CredencialOcrResultDto> ExtractCredencialDataAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                var result = new CredencialOcrResultDto();

                try
                {
                    using var engine = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default);
                    using var img = Pix.LoadFromFile(imagePath);
                    using var page = engine.Process(img);
                    var text = page.GetText();

                    _logger.LogInformation("OCR text extracted from image");

                    result.Afiliacion = ExtractAfiliacion(text);
                    result.Folio = ExtractFolio(text);
                    result.Pensionado = ExtractPensionado(text);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during OCR processing");
                    throw;
                }

                return result;
            });
        }

        private static string? ExtractAfiliacion(string text)
        {
            var patterns = new[]
            {
                @"Afiliaci[oó]n[:\s]*([A-Za-z0-9\-]+)",
                @"AFILIACI[OÓ]N[:\s]*([A-Za-z0-9\-]+)"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return null;
        }

        private static string? ExtractFolio(string text)
        {
            var patterns = new[]
            {
                @"Folio[:\s]*([A-Za-z0-9\-]+)",
                @"FOLIO[:\s]*([A-Za-z0-9\-]+)"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return null;
        }

        private static string? ExtractPensionado(string text)
        {
            var patterns = new[]
            {
                @"Pensionado[:\s]*([A-Za-z0-9\-\s]+)",
                @"PENSIONADO[:\s]*([A-Za-z0-9\-\s]+)",
                @"Pensi[oó]n[:\s]*([A-Za-z0-9\-\s]+)",
                @"PENSI[OÓ]N[:\s]*([A-Za-z0-9\-\s]+)"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    var value = match.Groups[1].Value.Trim();
                    // Limit to reasonable length and clean up
                    var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return string.Join(" ", words.Take(5));
                }
            }

            return null;
        }
    }
}
