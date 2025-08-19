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
        public async Task<byte[]> GenerateQrCodeWithLogoAsync(LoginData data, string logoPath, string outputPath, bool circularLogo = true)
        {
            string content = $"{data.Email}|{data.Password}|{data.CreatedDate:yyyy-MM-ddTHH:mm:ss}";
            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(content, QRCodeGenerator.ECCLevel.L);
            var qrCode = new PngByteQRCode(qrData);
            byte[] rawQr = qrCode.GetGraphic(5);

            using var qrImage = Image.Load<Rgba32>(rawQr);
            using var logo = await Image.LoadAsync<Rgba32>(logoPath);
            int logoSize = qrImage.Width / 5;

            logo.Mutate(x =>
            {
                x.Resize(logoSize, logoSize);
                if (circularLogo)
                {
                    var mask = new EllipsePolygon(logo.Width / 2f, logo.Height / 2f, logo.Width / 2f);
                    x.ApplyRoundedCorners(mask);
                }
            });

            qrImage.Mutate(x => x.DrawImage(logo, new Point((qrImage.Width - logoSize) / 2, (qrImage.Height - logoSize) / 2), 1f));

            //  Add header
            string headerText = "Scan to Login";
            string footerText = "© 2025 - Novel Wave Tech";
            int paddingTop = 20;
            int paddingBottom = 20;
            int textHeight = 60; // height allocated for each text section

            // === LOAD FONT ===
            var font = SystemFonts.CreateFont("Arial", 20, FontStyle.Bold);
            var font2 = SystemFonts.CreateFont("Arial", 12, FontStyle.Bold);

            // === TOTAL FINAL IMAGE SIZE ===
            int finalHeight = qrImage.Height + textHeight + textHeight + paddingTop + paddingBottom;

            // === CREATE FINAL IMAGE ===
            using var finalImage = new Image<Rgba32>(qrImage.Width, finalHeight);
            finalImage.Mutate(ctx =>
            {
                ctx.Fill(Color.White);

                // HEADER
                var headerOptions = new RichTextOptions(font)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Origin = new PointF(qrImage.Width / 2f, paddingTop)
                };
                ctx.DrawText(headerOptions, headerText, Color.Black);

                // QR CODE (centered vertically)
                ctx.DrawImage(qrImage, new Point(0, textHeight + paddingTop), 1f);

                // FOOTER
                var footerOptions = new RichTextOptions(font2)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Origin = new PointF(qrImage.Width / 2f, finalHeight - textHeight)
                };
                ctx.DrawText(footerOptions, footerText, Color.Black);
            });

            await using var ms = new MemoryStream();
            await finalImage.SaveAsPngAsync(ms);
            byte[] result = ms.ToArray();

            if (result.Length > 50 * 1024)
                throw new Exception("QR code with logo and header exceeds 50 KB. Reduce content or logo size.");

            await File.WriteAllBytesAsync(outputPath, result);
            return result;
        }

        public string ToBase64Image(byte[] qrBytes)
        {
            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }

    }
}
