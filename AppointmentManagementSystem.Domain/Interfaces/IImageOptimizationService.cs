using AppointmentManagementSystem.Domain.Models;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// Provides utilities for optimizing image payloads before persisting them.
    /// </summary>
    public interface IImageOptimizationService
    {
        /// <summary>
        /// Downscales and re-encodes an image to reduce its size while keeping reasonable quality.
        /// Accepts raw Base64 strings or full data URLs.
        /// </summary>
        /// <param name="base64Data">Base64 encoded image (data URL prefix allowed).</param>
        /// <param name="preferredContentType">Optional mime type hint for encoding.</param>
        /// <returns>Optimized image information.</returns>
        OptimizedImageResult OptimizeImage(string base64Data, string? preferredContentType = null);
    }
}
