using System;
using System.IO;
using System.Text.Json;
using SkiaSharp;

namespace GreenDragonTranscoder.Core.Services.SlateService
{
    public static class SlateService
    {
        public static void CreateSlate(SlateOptions options, string outputFilePath)
        {
            if (options.SlateInfo == null || string.IsNullOrEmpty(options.SlateBackgroundPath) || !File.Exists(options.SlateBackgroundPath))
            {
                throw new ArgumentException("Invalid input parameters or background image file does not exist.");
            }

            if (string.IsNullOrEmpty(options.SlateDrawingConfigPath) || !File.Exists(options.SlateDrawingConfigPath))
            {
                throw new ArgumentException("Invalid input parameters or slateDrawingConfig file does not exist.");
            }

            using var backgroundBitmap = TiffHelper.DecodeTiff(options.SlateBackgroundPath); //SKBitmap.Decode(slateBackgroundPath);
            using var surface = SKSurface.Create(new SKImageInfo(backgroundBitmap.Width, backgroundBitmap.Height));
            var canvas = surface.Canvas;
            // Draw the background image
            canvas.DrawBitmap(backgroundBitmap, 0, 0);
           
            var font = SKTypeface.FromFile(options.FontFile);

            // Set up paint for text
            var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#959595"),
                TextSize = 30.0f,
                Typeface = font,
                IsAntialias = true,
                TextAlign = SKTextAlign.Left, 
            };

            var drawingConfig = LoadDrawingConfig(options.SlateDrawingConfigPath);

            // Iterate over properties of SlateInfo and draw the text on the canvas
            foreach (var property in typeof(SlateInfo).GetProperties())
            {
                string propertyName = property.Name.ToLower();
                object propertyValue = property.GetValue(options.SlateInfo);

                if (drawingConfig.TryGetValue(propertyName, out TextPositionsConfig textConfig) && textConfig != null)
                {
                    if (propertyName == "title")
                    {
                        textPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 20);
                        DrawTextOnCanvas(canvas, $"{propertyValue}", textConfig, textPaint);
                    }

                    textPaint.MaskFilter = null;
                    DrawTextOnCanvas(canvas, $"{propertyValue}", textConfig, textPaint);
                }
            } 

            // Save the canvas to the memory stream
            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (FileStream fileStream = File.OpenWrite(outputFilePath))
            {
                data.SaveTo(fileStream);
            }
        }
        private static Dictionary<string, TextPositionsConfig> LoadDrawingConfig(string configFilePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            string json = File.ReadAllText(configFilePath);

            var config = JsonSerializer.Deserialize<Dictionary<string, TextPositionsConfig>>(json, options);

            if (config == null)
                return null;

            return config.ToDictionary(kvp => kvp.Key.ToLower(), kvp => kvp.Value);
        }

        private static void DrawTextOnCanvas(SKCanvas canvas, string text, TextPositionsConfig textConfig, SKPaint textPaint)
        {
            textPaint.TextSize = textConfig.FontSize;
            canvas.DrawText(text, textConfig.X, textConfig.Y, textPaint);
        }
    }
}
