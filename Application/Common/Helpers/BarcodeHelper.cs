using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.PDF417;
using ZXing.Datamatrix;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Core.Enums;
using Application.Dtos;

namespace Application.Common.Helpers
{
    public static class BarcodeHelper
    {
        public static string GenerateByHelper(BarcodeRequestDto data)
        {
            return data.BarcodeType switch
            {
                BarcodeType.Code128 => Code128(data.Content, data.Size),
                BarcodeType.Code39 => Code39(data.Content, data.Size),
                BarcodeType.Ean13 => Ean13(data.Content, data.Size),
                BarcodeType.Ean8 => Ean8(data.Content, data.Size),
                BarcodeType.QrCode => QrCode(data.Content, data.Size),
                BarcodeType.Pdf417 => Pdf417(data.Content, data.Size),
                BarcodeType.DataMatrix => DataMatrix(data.Content, data.Size),
                _ => throw new NotSupportedException("Unsupported symbology")
            };
        }

        public static string Code128(string content, BarcodeSize preset = BarcodeSize.Medium, int margin = 2, bool pure = true)
        {
            var (w, h) = ResolveLinearSize(preset);
            return ToBase64(Write(content, BarcodeFormat.CODE_128, w, h, margin, pure));
        }

        public static string Code39(string content, BarcodeSize preset = BarcodeSize.Medium, int margin = 2)
        {
            var (w, h) = ResolveLinearSize(preset);
            return ToBase64(Write(content, BarcodeFormat.CODE_39, w, h, margin));
        }

        public static string Ean13(string digits12, BarcodeSize preset = BarcodeSize.Medium, int margin = 2)
        {
            var (w, h) = ResolveLinearSize(preset);
            if (digits12.Length != 12 || !digits12.All(char.IsDigit))
                throw new ArgumentException("EAN13 requires 12 digits (without checksum).");
            var full = digits12 + CalcEan13Checksum(digits12);
            return ToBase64(Write(full, BarcodeFormat.EAN_13, w, h, margin));
        }

        public static string Ean8(string digits7, BarcodeSize preset = BarcodeSize.Medium, int margin = 2)
        {
            var (w, h) = ResolveLinearSize(preset);
            if (digits7.Length != 7 || !digits7.All(char.IsDigit))
                throw new ArgumentException("EAN8 requires 7 digits (without checksum).");
            var full = digits7 + CalcEan8Checksum(digits7);
            return ToBase64(Write(full, BarcodeFormat.EAN_8, w, h, margin));
        }

        public static string QrCode(string content, BarcodeSize preset = BarcodeSize.Medium, int margin = 1,
            ZXing.QrCode.Internal.ErrorCorrectionLevel? ecc = null)
        {
            var size = ResolveSquareSize(preset);
            var options = new QrCodeEncodingOptions
            {
                Width = size,
                Height = size,
                Margin = margin,
                CharacterSet = "UTF-8"
            };
            if (ecc != null) options.ErrorCorrection = ecc;
            return ToBase64(Write(content, BarcodeFormat.QR_CODE, options));
        }

        public static string Pdf417(string content, BarcodeSize preset = BarcodeSize.Medium, int margin = 2)
        {
            var (w, h) = ResolveLinearSize(preset);
            var options = new PDF417EncodingOptions
            {
                Margin = margin,
                Width = w,
                Height = h
            };
            return ToBase64(Write(content, BarcodeFormat.PDF_417, options));
        }

        public static string DataMatrix(string content, BarcodeSize preset = BarcodeSize.Small, int margin = 2)
        {
            var size = ResolveSquareSize(preset);
            var options = new DatamatrixEncodingOptions
            {
                Width = size,
                Height = size,
                Margin = margin
            };
            return ToBase64(Write(content, BarcodeFormat.DATA_MATRIX, options));
        }

        // Bytes (ví dụ dùng để trả file)
        public static byte[] Code128Bytes(string content, BarcodeSize preset = BarcodeSize.Medium)
        {
            var (w, h) = ResolveLinearSize(preset);
            return ToBytes(Write(content, BarcodeFormat.CODE_128, w, h));
        }

        // -------- Core Write Helpers --------
        private static ZXing.Rendering.PixelData Write(string content, BarcodeFormat format, int width, int height, int margin = 2, bool pureBarcode = false)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content empty");
            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = margin,
                    PureBarcode = pureBarcode
                }
            };
            return writer.Write(content);
        }

        private static ZXing.Rendering.PixelData Write(string content, BarcodeFormat format, EncodingOptions options)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content empty");
            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = options
            };
            return writer.Write(content);
        }

        // -------- Image Conversion --------
        private static string ToBase64(ZXing.Rendering.PixelData pixelData)
        {
            using Image<Rgba32> img = Image.LoadPixelData<Rgba32>(pixelData.Pixels, pixelData.Width, pixelData.Height);
            using var ms = new MemoryStream();
            img.Save(ms, new PngEncoder());
            return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        private static byte[] ToBytes(ZXing.Rendering.PixelData pixelData)
        {
            using Image<Rgba32> img = Image.LoadPixelData<Rgba32>(pixelData.Pixels, pixelData.Width, pixelData.Height);
            using var ms = new MemoryStream();
            img.Save(ms, new PngEncoder());
            return ms.ToArray();
        }

        // -------- Size Mapping --------
        private static (int w, int h) ResolveLinearSize(BarcodeSize preset) => preset switch
        {
            BarcodeSize.Tiny => (140, 40),
            BarcodeSize.Small => (220, 70),
            BarcodeSize.Medium => (300, 100),
            BarcodeSize.Large => (420, 140),
            _ => (300, 100)
        };

        private static int ResolveSquareSize(BarcodeSize preset) => preset switch
        {
            BarcodeSize.Tiny => 120,
            BarcodeSize.Small => 180,
            BarcodeSize.Medium => 250,
            BarcodeSize.Large => 340,
            _ => 250
        };

        // -------- Checksums --------
        private static char CalcEan13Checksum(string twelve)
        {
            int sumOdd = 0, sumEven = 0;
            for (int i = 0; i < 12; i++)
            {
                int d = twelve[i] - '0';
                if ((i & 1) == 0) sumOdd += d; else sumEven += d;
            }
            var total = sumOdd + sumEven * 3;
            var check = (10 - (total % 10)) % 10;
            return (char)('0' + check);
        }

        private static char CalcEan8Checksum(string seven)
        {
            int sumOdd = 0, sumEven = 0;
            for (int i = 0; i < 7; i++)
            {
                int d = seven[i] - '0';
                if ((i & 1) == 0) sumOdd += d; else sumEven += d;
            }
            var total = sumOdd * 3 + sumEven;
            var check = (10 - (total % 10)) % 10;
            return (char)('0' + check);
        }
    }
}