using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SkiaSharp;

namespace GreenDragonTranscoder.MauiCore.Services.SlateService
{
    public static class SlateService
    {
        public static void CreateSlate(SlateOptions options, string outputFilePath)
        {
            ValidateOptions(options);

            if (options.SlateBackground == null)
            {
                throw new ArgumentNullException(nameof(options.SlateBackground));   
            }

            using var backgroundBitmap = TiffHelper.DecodeTiff(options.SlateBackground);

            if (backgroundBitmap == null) 
            {
                throw new Exception("Couldn't decode tiff file.");
            }

            using var surface = SKSurface.Create(new SKImageInfo(backgroundBitmap.Width, backgroundBitmap.Height));
            var canvas = surface.Canvas;

            canvas.DrawBitmap(backgroundBitmap, 0, 0);

            var font = SKTypeface.FromStream(options.FontFile);
            var textPaint = CreateTextPaint(font);

            var drawingConfig = LoadDrawingConfig(options.SlateDrawingConfig);

            if (drawingConfig == null) 
            {
                throw new Exception("Invalid slate drawing config.");
            }

            foreach (var property in typeof(SlateInfo).GetProperties())
            {
                string propertyName = property.Name.ToLower();
                object? propertyValue = property?.GetValue(options.SlateInfo);

                if (drawingConfig.TryGetValue(propertyName, out TextPositionsConfig? textConfig) && textConfig != null)
                {
                    if (propertyName == "title")
                    {
                        // Draw back shadow for title
                        textPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 20);
                        DrawTextOnCanvas(canvas, $"{propertyValue}", textConfig, textPaint);
                    }

                    textPaint.MaskFilter = null;
                    DrawTextOnCanvas(canvas, $"{propertyValue}", textConfig, textPaint);
                }
            }

            SaveCanvasToFile(surface, outputFilePath);
        }

        private static void ValidateOptions(SlateOptions options)
        {
            if (options.SlateInfo == null || options.SlateBackground == null)
            {
                throw new ArgumentException("Invalid input parameters or background image file does not exist.");
            }

            if (options.SlateDrawingConfig == null)
            {
                throw new ArgumentException("Invalid input parameters or slateDrawingConfig file does not exist.");
            }
        }

        private static SKPaint CreateTextPaint(SKTypeface font)
        {
            return new SKPaint
            {
                Color = SKColor.Parse("#959595"),
                TextSize = 30.0f,
                Typeface = font,
                IsAntialias = true,
                TextAlign = SKTextAlign.Left,
            };
        }

        private static void DrawTextOnCanvas(SKCanvas canvas, string text, TextPositionsConfig textConfig, SKPaint textPaint)
        {
            textPaint.TextSize = textConfig.FontSize;
            canvas.DrawText(text, textConfig.X, textConfig.Y, textPaint);
        }

        private static void SaveCanvasToFile(SKSurface surface, string outputFilePath)
        {
            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (FileStream fileStream = File.OpenWrite(outputFilePath))
            {
                data.SaveTo(fileStream);
            }
        }

        private static Dictionary<string, TextPositionsConfig>? LoadDrawingConfig(Stream? configFile)
        {
            if (configFile == null)
            {
                // Handle the case where the stream is null
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            using (var streamReader = new StreamReader(configFile))
            {
                // Read the JSON content from the stream
                string json = streamReader.ReadToEnd();

                // Deserialize the JSON content into a Dictionary
                var config = JsonSerializer.Deserialize<Dictionary<string, TextPositionsConfig>>(json, options);

                // Convert the keys to lowercase and return the result
                return config?.ToDictionary(kvp => kvp.Key.ToLower(), kvp => kvp.Value);
            }
        }
    }
}
