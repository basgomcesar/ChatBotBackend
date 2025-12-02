using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IPE.Chatbot.Application.Features.Credencial.DTOs;
using IPE.Chatbot.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Tesseract;

namespace IPE.Chatbot.Api.Services
{
    public class OcrService : IOcrService
    {
        private readonly ILogger<OcrService> _logger;
        private readonly string _tessDataPath;

        // Configuraciones recomendadas (ajustables)
        private const int TargetMinDimension = 1600;   // si la dimensión mayor < esto, escalar
        private const float ContrastMultiplier = 1.25f;
        private const float SharpenSigma = 0.8f;
        private const float BlurSigma = 0.8f;         // para reducción de ruido previa a binarización
        private const int AdaptiveBlockSize = 25;     // debe ser impar, usar 21-35 según iluminación
        private const int AdaptiveC = 10;             // valor a restar de la media en threshold adaptivo
        private const int MaxDimensionForScaling = 2200;

        public OcrService(ILogger<OcrService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _tessDataPath = configuration["TesseractDataPath"] ?? "./tessdata";
        }

        // Método principal adaptado para seguir la lógica del código anterior:
        // - ejecutar OCR probando múltiples rotaciones,
        // - limpiar texto,
        // - aplicar variantes tolerantes de regex (incluyendo casos "Añiliación" / "«sión"),
        // - fallback para folio de 6 dígitos,
        // - segunda pasada localizada para Afiliación (whitelist numérica).
        public async Task<CredencialOcrResultDto> ExtractCredencialDataAsync(string imagePath)
        {
            var result = new CredencialOcrResultDto
            {
                Afiliacion = null,
                Folio = null,
                Pensionado = null
            };

            string preprocessedPath = null!;
            try
            {
                // 1) Obtener el mejor texto OCR probando rotaciones
                var text = await PerformOcrMultipleRotationsAsync(imagePath);
                _logger.LogDebug("Texto extraído (mejor rotación): {0}", text);

                // 2) Limpieza básica similar a la lógica previa en JS
                var textoLimpio = text
                    .Replace("\r", " ")
                    .Replace("\n", " ")
                    .Replace("“", "")
                    .Replace("”", "")
                    .Replace("‘", "")
                    .Replace("’", "")
                    .Replace("•", "")
                    .Replace("▪", "")
                    .Replace("*", "")
                    .Replace("|", "")
                    .Replace("'", "")
                    .Trim();

                textoLimpio = Regex.Replace(textoLimpio, @"\s{2,}", " ").Trim();

                string? afiliacion = null;

                // variante OCR mal leída: "Añiliación" en lugar de "Afiliación"
                var altAffMatch = Regex.Match(textoLimpio, @"[Aa]ñiliaci[oó]n\s*?(\d{4,10})", RegexOptions.IgnoreCase);
                if (altAffMatch.Success)
                    afiliacion = altAffMatch.Groups[1].Value;

                // variante donde OCR corta la palabra y queda algo como «sión o "sión"
                var altAffMatch2 = Regex.Match(textoLimpio, @"[«""]?s[ií]on\s*?(\d{4,10})", RegexOptions.IgnoreCase);
                if (string.IsNullOrEmpty(afiliacion) && altAffMatch2.Success)
                    afiliacion = altAffMatch2.Groups[1].Value;

                // extraer pensión (si aparece)
                var pensionMatch = Regex.Match(textoLimpio, @"Pensi[oó]n\s*(\d{4,6})", RegexOptions.IgnoreCase);
                var pension = pensionMatch.Success ? pensionMatch.Groups[1].Value : null;

                // 1) Buscar "filiación" bien escrito (con o sin acento)
                if (string.IsNullOrEmpty(afiliacion))
                {
                    var m = Regex.Match(textoLimpio, @"filiaci[oó]n[:\s]*([0-9]{4,10})", RegexOptions.IgnoreCase);
                    if (m.Success) afiliacion = m.Groups[1].Value;
                }

                // 2) Si no, buscar variante tolerante "afil+?aci…" (errores del OCR)
                if (string.IsNullOrEmpty(afiliacion))
                {
                    var m = Regex.Match(textoLimpio, @"afil+?aci[oó]?n?\D*([0-9]{4,10})", RegexOptions.IgnoreCase);
                    if (m.Success) afiliacion = m.Groups[1].Value;
                }

                // 3) Si aún no, buscar la terminación "lación" (p. ej. "lación 12345")
                if (string.IsNullOrEmpty(afiliacion))
                {
                    var m = Regex.Match(textoLimpio, @"(?:[fifl]+)?laci[oó]n[:\s]*([0-9]{4,10})", RegexOptions.IgnoreCase);
                    if (m.Success) afiliacion = m.Groups[1].Value;
                }

                // Buscar folio: preferir "Folio" o "Expedición"
                var folioMatch = Regex.Match(textoLimpio, @"(?:Folio|Expedic[ií][oó]n)[\s\S]*?(\d{6})", RegexOptions.IgnoreCase);
                string? folio = folioMatch.Success ? folioMatch.Groups[1].Value : null;

                // Fallback: cualquier secuencia de 6 dígitos que no coincida con afiliación o pensión
                if (string.IsNullOrEmpty(folio))
                {
                    var solo6 = Regex.Match(textoLimpio, @"\b(\d{6})\b");
                    if (solo6.Success)
                    {
                        var candidate = solo6.Groups[1].Value;
                        if (candidate != afiliacion && candidate != pension)
                            folio = candidate;
                    }
                }

                _logger.LogDebug("Afiliación detectada: {0}", afiliacion ?? "null");
                _logger.LogDebug("Pensión detectada: {0}", pension ?? "null");
                _logger.LogDebug("Folio detectado: {0}", folio ?? "null");

                result.Afiliacion = afiliacion;
                result.Pensionado = pension;
                result.Folio = folio;

                // 3) Si no encontramos afiliación, intentar segunda pasada localizada (zona superior-derecha)
                if (string.IsNullOrEmpty(result.Afiliacion))
                {
                    var regionPath = CropRegionForAffiliation(imagePath);
                    if (regionPath != null)
                    {
                        try
                        {
                            using var engineNum = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default);
                            engineNum.SetVariable("tessedit_char_whitelist", "0123456789");
                            using var imgNum = Pix.LoadFromFile(regionPath);
                            using var pageNum = engineNum.Process(imgNum, PageSegMode.SingleLine);
                            var numText = pageNum.GetText() ?? string.Empty;
                            var cleaned = CleanNumericLikeString(numText);
                            if (!string.IsNullOrWhiteSpace(cleaned))
                                result.Afiliacion = cleaned;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, "Error en segunda pasada numérica para Afiliación");
                        }
                        finally
                        {
                            try { if (File.Exists(regionPath)) File.Delete(regionPath); } catch { }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OCR processing");
                return result; // con valores tal como estén (posiblemente null)
            }
            finally
            {
                // No hay un preprocessedPath persistente aquí porque PerformOcrMultipleRotationsAsync maneja sus temporales.
            }
        }

        // Ejecuta OCR probando rotaciones 0,90,180,270 sobre la imagen preprocesada y devuelve el mejor texto.
        private async Task<string> PerformOcrMultipleRotationsAsync(string filePath)
        {
            // Ejecutamos el preprocesado en un hilo de trabajo para no bloquear el thread que llama
            var preprocessedPath = await Task.Run(() => PreprocessImage(filePath));

            try
            {
                byte[] originalBytes = await File.ReadAllBytesAsync(preprocessedPath);

                // Si el ancho es muy pequeño, escalar a 1000px de ancho para mejorar OCR
                using (var meta = Image.Load<Rgba32>(originalBytes))
                {
                    if (meta.Width < 500)
                    {
                        using var resized = meta.Clone(ctx => ctx.Resize(1000, 0, KnownResamplers.Lanczos3));
                        using var ms = new MemoryStream();
                        await resized.SaveAsPngAsync(ms);
                        originalBytes = ms.ToArray();
                    }
                }

                var rotations = new[] { 0, 90, 180, 270 };
                string bestText = string.Empty;
                float bestConfidence = -1f;

                foreach (var angle in rotations)
                {
                    // Rotar y ajustar tamaño a un máximo de 1000x1000 para consistencia
                    using var img = Image.Load<Rgba32>(originalBytes);
                    if (angle != 0) img.Mutate(x => x.Rotate((float)angle));
                    img.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(1000, 1000),
                        Sampler = KnownResamplers.Lanczos3
                    }));

                    byte[] rotatedBytes;
                    using (var ms = new MemoryStream())
                    {
                        await img.SaveAsPngAsync(ms);
                        rotatedBytes = ms.ToArray();
                    }

                    // OCR en hilo de fondo (Tesseract es síncrono y costoso)
                    var (extractedText, confidence) = await Task.Run(() =>
                    {
                        try
                        {
                            using var engine = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default);
                            using var pix = Pix.LoadFromMemory(rotatedBytes);
                            using var page = engine.Process(pix, PageSegMode.SingleBlock);
                            var txt = page.GetText() ?? string.Empty;
                            var conf = page.GetMeanConfidence(); // 0..1
                            return (txt.Trim(), conf);
                        }
                        catch (Exception e)
                        {
                            _logger.LogDebug(e, "Tesseract falló en rotación {Angle}", angle);
                            return (string.Empty, 0f);
                        }
                    });

                    var currentLen = string.IsNullOrEmpty(extractedText) ? 0 : extractedText.Length;
                    _logger.LogDebug("Rotación {Angle}° → Longitud texto: {Len}, Confianza: {Conf}", angle, currentLen, confidence);

                    // Criterio: preferir mayor longitud de texto y confianza >= anterior
                    if (currentLen > bestText.Length && confidence >= bestConfidence)
                    {
                        bestText = extractedText;
                        bestConfidence = confidence;
                    }
                    else if (bestText.Length == 0 && confidence > bestConfidence)
                    {
                        // caso inicial donde no hay texto pero hay confianza
                        bestText = extractedText;
                        bestConfidence = confidence;
                    }
                }

                return bestText;
            }
            finally
            {
                try { if (File.Exists(preprocessedPath)) File.Delete(preprocessedPath); } catch { }
            }
        }

        // ---------- Preprocesado con SixLabors.ImageSharp ----------
        // Pipeline aplicado:
        // - Escalado si es necesario (Lanczos3)
        // - Escala de grises
        // - Reducción ligera de ruido (GaussianBlur)
        // - Aumento de contraste
        // - Sharpen
        // - Binarización adaptativa por promedio local (blockSize = AdaptiveBlockSize, C = AdaptiveC)
        // - Guardado temporal PNG para Tesseract
        private string PreprocessImage(string imagePath)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"ocr_pre_{Guid.NewGuid():N}.png");

            using var image = Image.Load<Rgba32>(imagePath);

            // Escalar si la dimensión mayor es menor que TargetMinDimension
            var maxDim = Math.Max(image.Width, image.Height);
            if (maxDim < TargetMinDimension || maxDim > MaxDimensionForScaling)
            {
                var target = Math.Clamp(TargetMinDimension, TargetMinDimension, MaxDimensionForScaling);
                if (maxDim < TargetMinDimension) target = TargetMinDimension;
                if (maxDim > MaxDimensionForScaling) target = MaxDimensionForScaling;

                var scale = (float)target / maxDim;
                var newW = Math.Max(1, (int)Math.Round(image.Width * scale));
                var newH = Math.Max(1, (int)Math.Round(image.Height * scale));
                image.Mutate(x => x.Resize(newW, newH, KnownResamplers.Lanczos3));
            }

            // Convertir a escala de grises
            image.Mutate(x => x.Grayscale());

            // Reducción de ruido leve
            image.Mutate(x => x.GaussianBlur(BlurSigma));

            // Aumentar contraste y afilar
            image.Mutate(x => x.Contrast(ContrastMultiplier));
            image.Mutate(x => x.GaussianSharpen(SharpenSigma));

            // Obtener luminancias en 2D para threshold adaptativo
            var width = image.Width;
            var height = image.Height;
            var lum = new byte[width * height];
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        lum[y * width + x] = row[x].R;
                    }
                }
            });

            // Aplicar threshold adaptativo por bloques (media local - C). Si algo falla, usar Otsu como fallback.
            var binary = new byte[width * height];
            var block = AdaptiveBlockSize;
            if (block % 2 == 0) block++; // asegurar impar

            try
            {
                ApplyAdaptiveMeanThreshold(lum, binary, width, height, block, AdaptiveC);
            }
            catch (Exception)
            {
                var otsu = ComputeOtsuThreshold(lum);
                for (int i = 0; i < lum.Length; i++)
                    binary[i] = (byte)(lum[i] > otsu ? 255 : 0);
            }

            // Crear imagen binaria y guardarla
            using var outImg = new Image<L8>(width, height);
            outImg.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        row[x].PackedValue = binary[y * width + x];
                    }
                }
            });

            outImg.SaveAsPng(tempFile);

            return tempFile;
        }

        // Aplica threshold adaptativo por media local (media de bloque) y escribe 0/255 en salida
        private static void ApplyAdaptiveMeanThreshold(byte[] lum, byte[] outBin, int width, int height, int blockSize, int c)
        {
            var integral = new long[(width + 1) * (height + 1)];
            for (int y = 1; y <= height; y++)
            {
                long rowSum = 0;
                for (int x = 1; x <= width; x++)
                {
                    rowSum += lum[(y - 1) * width + (x - 1)];
                    integral[y * (width + 1) + x] = integral[(y - 1) * (width + 1) + x] + rowSum;
                }
            }

            int half = blockSize / 2;
            for (int y = 0; y < height; y++)
            {
                int y0 = Math.Max(0, y - half);
                int y1 = Math.Min(height - 1, y + half);
                for (int x = 0; x < width; x++)
                {
                    int x0 = Math.Max(0, x - half);
                    int x1 = Math.Min(width - 1, x + half);

                    long sum = integral[(y1 + 1) * (width + 1) + (x1 + 1)]
                             - integral[(y0) * (width + 1) + (x1 + 1)]
                             - integral[(y1 + 1) * (width + 1) + (x0)]
                             + integral[(y0) * (width + 1) + (x0)];

                    int count = (x1 - x0 + 1) * (y1 - y0 + 1);
                    int mean = (int)(sum / count);

                    var idx = y * width + x;
                    outBin[idx] = (byte)(lum[idx] > mean - c ? 255 : 0);
                }
            }
        }

        // Otsu implementado (usa luminancias 0..255)
        private static int ComputeOtsuThreshold(byte[] data)
        {
            if (data == null || data.Length == 0) return 128;

            var hist = new long[256];
            foreach (var b in data) hist[b]++;

            var total = data.Length;
            var sum = 0L;
            for (int t = 0; t < 256; t++) sum += t * hist[t];

            var sumB = 0L;
            var wB = 0L;
            var wF = 0L;

            double varMax = 0;
            int threshold = 0;

            for (int t = 0; t < 256; t++)
            {
                wB += hist[t];
                if (wB == 0) continue;
                wF = total - wB;
                if (wF == 0) break;

                sumB += (long)(t * hist[t]);
                var mB = (double)sumB / wB;
                var mF = (double)(sum - sumB) / wF;

                var between = (double)wB * (double)wF * (mB - mF) * (mB - mF);

                if (between > varMax)
                {
                    varMax = between;
                    threshold = t;
                }
            }

            return threshold;
        }

        // ---------- Recorte heurístico de zona de Afiliación (superior-derecha) ----------
        private string? CropRegionForAffiliation(string imagePath)
        {
            try
            {
                using var img = Image.Load<Rgba32>(imagePath);
                var w = img.Width;
                var h = img.Height;

                var x = (int)(w * 0.48);  // empezar cerca de mitad derecha
                var y = (int)(h * 0.04);  // top pequeño
                var cw = (int)(w * 0.5);  // mitad derecha
                var ch = (int)(h * 0.20); // zona superior

                x = Math.Clamp(x, 0, w - 1);
                y = Math.Clamp(y, 0, h - 1);
                cw = Math.Clamp(cw, 10, w - x);
                ch = Math.Clamp(ch, 10, h - y);

                var rectPath = Path.Combine(Path.GetTempPath(), $"ocr_crop_aff_{Guid.NewGuid():N}.png");
                using var crop = img.Clone(ctx => ctx.Crop(new Rectangle(x, y, cw, ch)));
                crop.SaveAsPng(rectPath);
                return rectPath;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "No se pudo recortar la región de Afiliación");
                return null;
            }
        }

        // ---------- Helpers ya presentes (se conservan para compatibilidad) ----------
        private static string? ExtractAfiliacion(string text)
        {
            var keywords = new[] { "afiliacion", "afiliación", "no afiliacion", "numero afiliacion", "n° afiliacion", "no. afiliacion", "no. de afiliacion" };
            return ExtractNumberNearKeywords(text, keywords, minDigits: 4, maxDigits: 12);
        }

        private static string? ExtractFolio(string text)
        {
            var keywords = new[] { "folio", "folio no", "no folio", "no. folio", "n° folio" };
            return ExtractNumberNearKeywords(text, keywords, minDigits: 3, maxDigits: 12);
        }

        private static string? ExtractPensionado(string text)
        {
            var keywords = new[] { "pensionado", "pensión", "pension", "no pensionado", "no. pensionado", "n° pensionado" };
            return ExtractNumberNearKeywords(text, keywords, minDigits: 3, maxDigits: 12);
        }

        private static string? ExtractNumberNearKeywords(string text, string[] keywords, int minDigits, int maxDigits)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var normalizedLine = NormalizeForComparison(lines[i]);
                if (keywords.Any(k => normalizedLine.Contains(NormalizeForComparison(k))))
                {
                    var num = ExtractDigitsFromString(lines[i], minDigits, maxDigits);
                    if (!string.IsNullOrEmpty(num)) return num;

                    for (int j = 1; j <= 2 && i + j < lines.Length; j++)
                    {
                        num = ExtractDigitsFromString(lines[i + j], minDigits, maxDigits);
                        if (!string.IsNullOrEmpty(num)) return num;
                    }

                    for (int j = 1; j <= 2 && i - j >= 0; j++)
                    {
                        var prevNum = ExtractDigitsFromString(lines[i - j], minDigits, maxDigits);
                        if (!string.IsNullOrEmpty(prevNum)) return prevNum;
                    }
                }
            }

            var labelPattern = string.Join("|", keywords.Select(Regex.Escape));
            var regexLabel = new Regex($@"(?:{labelPattern})[:\s\-]*([0-9\-\s]{{{minDigits},{maxDigits + 10}}})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            var match = regexLabel.Match(text);
            if (match.Success)
            {
                var candidate = ExtractDigitsFromString(match.Groups[1].Value, minDigits, maxDigits);
                if (!string.IsNullOrEmpty(candidate)) return candidate;
            }

            var fallback = FindBestDigitSequence(text, minDigits, maxDigits);
            return fallback;
        }

        private static string? ExtractDigitsFromString(string input, int minDigits, int maxDigits)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var cleaned = input.Replace("O", "0").Replace("o", "0")
                               .Replace("I", "1").Replace("l", "1").Replace("L", "1")
                               .Replace(" ", string.Empty)
                               .Replace(".", string.Empty)
                               .Replace("-", string.Empty)
                               .Replace("/", string.Empty)
                               .Replace(":", string.Empty);

            var digitMatches = Regex.Matches(cleaned, @"\d+");
            string? best = null;
            foreach (Match m in digitMatches)
            {
                var digits = m.Value;
                if (digits.Length >= minDigits && digits.Length <= maxDigits)
                {
                    if (best == null || digits.Length > best.Length) best = digits;
                }
            }

            if (best == null)
            {
                var allDigits = digitMatches.Cast<Match>().Select(x => x.Value).OrderByDescending(s => s.Length).FirstOrDefault();
                if (!string.IsNullOrEmpty(allDigits) && allDigits.Length >= minDigits) best = allDigits;
            }

            return best;
        }

        private static string? FindBestDigitSequence(string text, int minDigits, int maxDigits)
        {
            var cleaned = text.Replace(" ", string.Empty).Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty).Replace(":", string.Empty);
            cleaned = cleaned.Replace("O", "0").Replace("o", "0").Replace("I", "1").Replace("l", "1").Replace("L", "1");

            var matches = Regex.Matches(cleaned, @"\d+");
            if (matches.Count == 0) return null;

            var sorted = matches.Cast<Match>().Select(m => m.Value).OrderByDescending(s => s.Length).ToArray();

            foreach (var candidate in sorted)
            {
                if (candidate.Length >= minDigits && candidate.Length <= maxDigits) return candidate;
            }

            var first = sorted.First();
            return first.Length >= minDigits ? first : null;
        }

        private static string NormalizeForComparison(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var lower = input.ToLowerInvariant();

            var normalizedString = lower.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalizedString)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            var result = sb.ToString().Normalize(NormalizationForm.FormC);
            result = Regex.Replace(result, @"\s+", " ");
            result = Regex.Replace(result, @"[^a-z0-9ñ\s]", " ");
            return result.Trim();
        }

        private static string CleanNumericLikeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var s = input.Trim();
            s = s.Replace(" ", "").Replace(".", "").Replace("-", "").Replace("O", "0").Replace("o", "0").Replace("I", "1").Replace("l", "1").Replace("L", "1");
            var m = Regex.Match(s, @"\d{3,}");
            return m.Success ? m.Value : string.Empty;
        }
    }
}