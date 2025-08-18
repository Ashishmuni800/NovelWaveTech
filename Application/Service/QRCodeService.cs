using Application.DTO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Windows.Compatibility;
namespace Application.Service
{
    public class QRCodeService
    {
        public byte[] GenerateQrCode(LoginData data)
        {
            string content = $"{data.Email}|{data.Password}";

            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20); // You can change pixels-per-module

            return qrCodeBytes;
        }

        public byte[] GenerateAndSaveQrCode(LoginData data, string filePath)
        {
            string content = $"{data.Email}|{data.Password}";

            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            // Save the QR code as a PNG image
            File.WriteAllBytes(filePath, qrCodeBytes);

            return qrCodeBytes;
        }


        public LoginData DecodeQrCode(Stream imageStream)
        {
            using var bitmap = new Bitmap(imageStream);

            var reader = new BarcodeReader(); // Works now
            var result = reader.Decode(bitmap);

            if (result == null)
                throw new Exception("QR Code could not be read.");

            var parts = result.Text.Split('|');
            if (parts.Length != 2)
                throw new Exception("Invalid QR Code format.");

            return new LoginData
            {
                Email = parts[0],
                Password = parts[1]
            };
        }

    }
}

