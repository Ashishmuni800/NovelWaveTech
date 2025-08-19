using Application.DTO;
using QRCoder;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public static class ImageExtensions
    {
        public static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, IPath shape)
        {
            var options = new GraphicsOptions
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver,
                BlendPercentage = 1.0f
            };
            var transparent = new Rgba32(0, 0, 0, 0);
            var mask = new DrawingOptions { GraphicsOptions = options };

            ctx.SetGraphicsOptions(options);
            ctx.Fill(transparent);
            ctx.Fill(Color.White, shape);
            return ctx;
        }
    }
    public class QRCodeWithLogoService
    {
        public async Task<byte[]> GenerateQrCodeWithLogoAsync(LoginData data, string outputPath)
        {
            string content = $"{data.Email}|{data.Password}|{data.CreatedDate:yyyy-MM-ddTHH:mm:ss}";

            // Generate QR code with high ECC
            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(content, QRCodeGenerator.ECCLevel.H); // High error correction
            var qrCode = new PngByteQRCode(qrData);
            byte[] rawQr = qrCode.GetGraphic(8); // higher resolution

            using var qrImage = Image.Load<Rgba32>(rawQr);

            // === Draw custom center logo (text-based) ===
            qrImage.Mutate(ctx =>
            {
                int logoSize = qrImage.Width / 6; // Smaller logo
                float radius = logoSize / 2f;
                float centerX = qrImage.Width / 2f;
                float centerY = qrImage.Height / 2f;
                var logoCenter = new PointF(centerX, centerY);

                // Draw white border around logo
                ctx.Fill(Color.White, new EllipsePolygon(logoCenter, radius + 5)); // white margin to help decoding
                ctx.Fill(Color.BlueViolet, new EllipsePolygon(logoCenter, radius)); // main circle

                // Draw text inside the logo
                var centerFont = SystemFonts.CreateFont("Arial", 16, FontStyle.Bold);
                var centerTextOptions = new RichTextOptions(centerFont)
                {
                    Origin = logoCenter,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                ctx.DrawText(centerTextOptions, "NWT", Color.White);
            });

            // === Fonts and labels ===
            var brandFont = SystemFonts.CreateFont("Arial", 20, FontStyle.Bold);
            var labelFont = SystemFonts.CreateFont("Arial", 20, FontStyle.Regular);
            var labelFont2 = SystemFonts.CreateFont("Arial", 12, FontStyle.Regular);

            string brandText = "Novel Wave Tech";
            string headerText = "Scan to Login";
            string footerText = "© 2025 - Novel Wave Tech";

            // === Layout calculations ===
            int iconDiameter = 50;
            int spacing = 10;
            int textHeight = 40;
            int headerSectionHeight = iconDiameter + spacing + textHeight * 2 + 20;
            int footerSectionHeight = textHeight + 20;
            int finalHeight = headerSectionHeight + qrImage.Height + footerSectionHeight;

            using var finalImage = new Image<Rgba32>(qrImage.Width, finalHeight);
            finalImage.Mutate(ctx =>
            {
                ctx.Fill(Color.White);
                float centerX = qrImage.Width / 2f;
                var logoCenter = new PointF(centerX, iconDiameter / 2f + 10);

                // Header circle logo with 'N'
                ctx.Fill(Color.BlueViolet, new EllipsePolygon(logoCenter, iconDiameter / 2f));
                var initialFont = SystemFonts.CreateFont("Arial", 24, FontStyle.Bold);
                var initialOptions = new RichTextOptions(initialFont)
                {
                    Origin = logoCenter,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                ctx.DrawText(initialOptions, "N", Color.White);

                // Brand text
                var brandOptions = new RichTextOptions(brandFont)
                {
                    Origin = new PointF(centerX, logoCenter.Y + iconDiameter / 2f + 5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                ctx.DrawText(brandOptions, brandText, Color.Black);

                // Header label
                var headerOptions = new RichTextOptions(labelFont)
                {
                    Origin = new PointF(centerX, logoCenter.Y + iconDiameter / 2f + textHeight + 5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                ctx.DrawText(headerOptions, headerText, Color.Gray);

                // QR code (already contains center logo)
                ctx.DrawImage(qrImage, new Point(0, headerSectionHeight), 1f);

                // Footer text
                var footerOptions = new RichTextOptions(labelFont2)
                {
                    Origin = new PointF(centerX, finalHeight - textHeight - 10),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                ctx.DrawText(footerOptions, footerText, Color.Black);
            });

            await using var ms = new MemoryStream();
            await finalImage.SaveAsPngAsync(ms);
            byte[] result = ms.ToArray();

            if (result.Length > 50 * 1024)
                throw new Exception("Final image exceeds 50 KB. Try reducing logo or QR resolution.");

            await File.WriteAllBytesAsync(outputPath, result);
            return result;
        }





        public string ToBase64Image(byte[] qrBytes)
        {
            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }

    }
}
