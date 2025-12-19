using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IPE.Chatbot.Application.Features.Credencial.DTOs;
using IPE.Chatbot.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Tesseract;

namespace IPE.Chatbot.Api.Services {
    public class OcrService : IOcrService {
        private readonly ILogger<OcrService> _logger;
        private readonly string _tessDataPath;

        private const int TargetMinDimension = 1800;
        private const float ContrastMultiplier = 2.0f;
        private const float SharpenSigma = 0.8f;

        public OcrService(ILogger<OcrService> logger, IWebHostEnvironment env) {
            _logger = logger;
            _tessDataPath = Path.Combine(env.ContentRootPath, "tessdata");
        }

        public async Task<CredencialOcrResultDto> ExtractCredencialDataAsync(string imagePath) {
            var result = new CredencialOcrResultDto();

            try {
                var text = await PerformDualPassOcrAsync(imagePath);
                _logger.LogInformation("OCR RAW >>>\n{Text}", text);

                MapFields(result, text);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error during OCR processing");
            }

            return result;
        }

        // ==============================
        // OCR EN DOS PASADAS
        // ==============================
        private async Task<string> PerformDualPassOcrAsync(string imagePath) {
            var preprocessedPath = PreprocessImage(imagePath);

            try {
                byte[] bytes = await File.ReadAllBytesAsync(preprocessedPath);
                var allText = string.Empty;

                using var engineText = new TesseractEngine(_tessDataPath, "spa", EngineMode.LstmOnly);
                engineText.DefaultPageSegMode = PageSegMode.Auto;

                using var pix = Pix.LoadFromMemory(bytes);
                using var pageText = engineText.Process(pix);
                allText += pageText.GetText() + "\n";

                using var engineDigits = new TesseractEngine(_tessDataPath, "spa", EngineMode.LstmOnly);
                engineDigits.SetVariable("tessedit_char_whitelist", "0123456789");
                engineDigits.DefaultPageSegMode = PageSegMode.SparseText;

                using var pageDigits = engineDigits.Process(pix);
                allText += pageDigits.GetText();

                return allText;
            } finally {
                try { if (File.Exists(preprocessedPath)) File.Delete(preprocessedPath); } catch { }
            }
        }

        // ==============================
        // PREPROCESAMIENTO
        // ==============================
        private string PreprocessImage(string imagePath) {
            var tempFile = Path.Combine(Path.GetTempPath(), $"ocr_pre_{Guid.NewGuid():N}.png");

            using var image = Image.Load<Rgba32>(imagePath);

            var maxDim = Math.Max(image.Width, image.Height);
            if (maxDim < TargetMinDimension) {
                var scale = (float)TargetMinDimension / maxDim;
                image.Mutate(x => x.Resize((int)(image.Width * scale), (int)(image.Height * scale)));
            }

            image.Mutate(x => x
                .Grayscale()
                .Contrast(ContrastMultiplier)
                .GaussianSharpen(SharpenSigma)
            );

            image.SaveAsPng(tempFile);
            return tempFile;
        }

        // ==============================
        // MAPEO DE CAMPOS (LÓGICA CORREGIDA)
        // ==============================
        private void MapFields(CredencialOcrResultDto dto, string text) {
            if (string.IsNullOrWhiteSpace(text)) return;

            var limpio = Regex.Replace(text, @"\s+", " ");

            dto.Afiliacion = ExtraerNumeroCercano(limpio, "Afiliación", 6);
            dto.Pensionado = ExtraerNumeroCercano(limpio, "Pensión", 5);

            dto.Folio =
                ExtraerNumeroCercano(limpio, "Folio", 6, dto.Afiliacion ?? string.Empty, dto.Pensionado ?? string.Empty)
                ?? ExtraerNumeroCercano(limpio, "Expedición", 6, dto.Afiliacion ?? string.Empty, dto.Pensionado ?? string.Empty);
        }

        // ==============================
        // UTILIDAD: NÚMERO MÁS CERCANO A PALABRA CLAVE
        // ==============================
        private string? ExtraerNumeroCercano(string texto, string palabraClave, int digitos, params string[] excluir) {
            var idx = texto.IndexOf(palabraClave, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            var numeros = Regex.Matches(texto, $@"\b\d{{{digitos}}}\b")
                .Cast<Match>()
                .Select(m => new {
                    Valor = m.Value,
                    Distancia = Math.Abs(m.Index - idx)
                })
                .Where(x => !excluir.Contains(x.Valor))
                .OrderBy(x => x.Distancia)
                .FirstOrDefault();

            return numeros?.Valor;
        }
    }
}
