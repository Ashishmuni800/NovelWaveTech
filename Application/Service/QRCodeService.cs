using Application.DTO;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
//using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Windows.Compatibility;

namespace Application.Service
{

    public class QRCodeService
    {
        public byte[] GenerateQrCode(LoginData data)
        {
            string content = $"{data.Email}|{data.Password}|{data.CreatedDate}";

            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20); // You can change pixels-per-module

            return qrCodeBytes;
        }

        public byte[] GenerateAndSaveQrCode(LoginData data, string filePath)
        {
            string content = $"{data.Email}|{data.Password}|{data.CreatedDate}";

            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            // Save the QR code as a PNG image
            File.WriteAllBytes(filePath, qrCodeBytes);

            return qrCodeBytes;
        }


        //public LoginData DecodeQrCode(Stream imageStream)
        //{
        //    using var bitmap = new Bitmap(imageStream);
        //    var reader = new BarcodeReader();

        //    var result = reader.Decode(bitmap);

        //    if (result == null)
        //        throw new Exception("QR Code could not be read.");

        //    var parts = result.Text.Split('|');

        //    if (parts.Length != 3)
        //        throw new Exception("Invalid QR Code format. Expected 3 parts: Email|Password|CreatedDate");

        //    if (!DateTime.TryParse(parts[2], out var createdDate))
        //        throw new Exception("Invalid date format in QR Code.");

        //    if (DateTime.UtcNow > createdDate.AddHours(24))
        //        throw new Exception("QR Code is expired.");

        //    return new LoginData
        //    {
        //        Email = parts[0],
        //        Password = parts[1],
        //        CreatedDate = createdDate // Or parse to DateTime if needed
        //    };
        //}
        public LoginData DecodeQrCode(Stream imageStream)
        {
            // Load the image as a bitmap (ZXing works with System.Drawing.Bitmap)
            using var bitmap = new Bitmap(imageStream);

            var reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new[] { ZXing.BarcodeFormat.QR_CODE },
                    PureBarcode = false
                }
            };

            var result = reader.Decode(bitmap);
            if (result == null)
                throw new Exception("QR Code could not be read. Please ensure it's clear and not obstructed.");

            var parts = result.Text.Split('|');
            if (parts.Length != 3)
                throw new Exception("Invalid QR Code format. Expected: Email|Password|CreatedDate");

            if (!DateTime.TryParse(parts[2], out var createdDate))
                throw new Exception("Invalid date format in QR Code.");

            if (DateTime.UtcNow > createdDate.AddHours(24))
                throw new Exception("QR Code has expired. It was generated over 24 hours ago.");

            return new LoginData
            {
                Email = parts[0],
                Password = parts[1],
                CreatedDate = createdDate
            };
        }

    private Bitmap ImageSharpToBitmap(Image<Rgba32> image)
        {
            using var ms = new MemoryStream();
            image.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return new Bitmap(ms);
        }
        //    public byte[] GenerateQrCodeWithLogo(LoginData data, string filePath, string logoPath, bool cropLogoToCircle = true)
        //    {
        //        string content = $"{data.Email}|{data.Password}|{data.CreatedDate:yyyy-MM-ddTHH:mm:ss}";
        //        using QRCodeGenerator qrGenerator = new();
        //        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.L);
        //        using QRCode qrCode = new(qrCodeData);

        //        int pixelsPerModule = 5;
        //        byte[] qrBytes = null;

        //        while (pixelsPerModule >= 1)
        //        {
        //            using Bitmap qrBitmap = qrCode.GetGraphic(pixelsPerModule);
        //            using Bitmap logo = new Bitmap(logoPath);

        //            using Bitmap processedLogo = cropLogoToCircle ? CropToCircle(logo) : new Bitmap(logo);
        //            using Bitmap qrWithLogo = AddLogoToQr(qrBitmap, processedLogo);

        //            using MemoryStream ms = new();
        //            var encoder = GetEncoder(ImageFormat.Png);
        //            var encoderParams = new EncoderParameters(1);
        //            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);

        //            qrWithLogo.Save(ms, encoder, encoderParams);
        //            qrBytes = ms.ToArray();

        //            if (qrBytes.Length <= 50 * 1024)
        //            {
        //                File.WriteAllBytes(filePath, qrBytes);
        //                return qrBytes;
        //            }

        //            pixelsPerModule--;
        //        }

        //        throw new Exception("QR Code with logo exceeds 50 KB. Try smaller content or logo.");
        //    }
        //    private Bitmap AddLogoToQr(Bitmap qrCode, Bitmap logo)
        //    {
        //        int overlaySize = qrCode.Width / 5; // 20% of QR code size
        //        Bitmap resizedLogo = new Bitmap(logo, new Size(overlaySize, overlaySize));

        //        Bitmap combined = new Bitmap(qrCode.Width, qrCode.Height, PixelFormat.Format32bppArgb);
        //        using Graphics g = Graphics.FromImage(combined)
        //{
        //            g.CompositingMode = CompositingMode.SourceOver;
        //            g.SmoothingMode = SmoothingMode.AntiAlias;
        //            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //            g.DrawImage(qrCode, 0, 0);
        //            g.DrawImage(resizedLogo, (qrCode.Width - overlaySize) / 2, (qrCode.Height - overlaySize) / 2, overlaySize, overlaySize);
        //        }

        //        return combined;
        //    }
        //    private Bitmap CropToCircle(Bitmap logo)
        //    {
        //        int size = Math.Min(logo.Width, logo.Height);
        //        Bitmap circleLogo = new Bitmap(size, size, PixelFormat.Format32bppArgb);

        //        using (Graphics g = Graphics.FromImage(circleLogo))
        //        {
        //            using (GraphicsPath path = new GraphicsPath())
        //            {
        //                path.AddEllipse(0, 0, size, size);
        //                g.SetClip(path);
        //                g.DrawImage(logo, new Rectangle(0, 0, size, size));
        //            }
        //        }

        //        return circleLogo;
        //    }
        //    public string ConvertToBase64(byte[] imageBytes)
        //    {
        //        return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        //    }
    }
}

