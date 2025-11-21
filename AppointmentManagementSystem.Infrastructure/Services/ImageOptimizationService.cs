using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Domain.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace AppointmentManagementSystem.Infrastructure.Services
{
    public class ImageOptimizationService : IImageOptimizationService
    {
        private const int MaxWidth = 1280;
        private const int MaxHeight = 1280;
        private const int JpegQuality = 75;

        public OptimizedImageResult OptimizeImage(string base64Data, string? preferredContentType = null)
        {
            if (string.IsNullOrWhiteSpace(base64Data))
            {
                return new OptimizedImageResult
                {
                    Base64Data = string.Empty,
                    ContentType = preferredContentType ?? "image/jpeg",
                    FileSize = 0
                };
            }

            var (normalizedBase64, _) = NormalizeBase64(base64Data);

            try
            {
                var imageBytes = Convert.FromBase64String(normalizedBase64);

                using var image = Image.Load(imageBytes);

                var resizeRatio = Math.Min((double)MaxWidth / image.Width, (double)MaxHeight / image.Height);
                if (resizeRatio < 1)
                {
                    var targetWidth = (int)Math.Round(image.Width * resizeRatio);
                    var targetHeight = (int)Math.Round(image.Height * resizeRatio);
                    image.Mutate(x => x.Resize(targetWidth, targetHeight));
                }

                using var ms = new MemoryStream();
                var encoder = new JpegEncoder { Quality = JpegQuality };
                image.Save(ms, encoder);

                var optimizedBytes = ms.ToArray();

                var contentType = "image/jpeg";

                return new OptimizedImageResult
                {
                    Base64Data = Convert.ToBase64String(optimizedBytes),
                    ContentType = contentType,
                    FileSize = optimizedBytes.Length
                };
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Geçersiz Base64 görüntü verisi.", ex);
            }
        }

        private static (string Base64, string? ContentType) NormalizeBase64(string base64Data)
        {
            var markerIndex = base64Data.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
            if (markerIndex >= 0)
            {
                var prefix = base64Data[..markerIndex];
                string? contentType = null;

                var dataIndex = prefix.IndexOf("data:", StringComparison.OrdinalIgnoreCase);
                if (dataIndex >= 0)
                {
                    var typeSegment = prefix[(dataIndex + 5)..];
                    var separatorIndex = typeSegment.IndexOf(';');
                    contentType = separatorIndex >= 0
                        ? typeSegment[..separatorIndex]
                        : typeSegment;
                }

                return (base64Data[(markerIndex + "base64,".Length)..], contentType);
            }

            return (base64Data, null);
        }
    }
}
