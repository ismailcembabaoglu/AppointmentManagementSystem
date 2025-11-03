using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace AppointmentManagementSystem.Maui.Services
{
    public interface ICameraService
    {
        Task<string?> TakePhotoAsync();
        Task<string?> PickPhotoAsync();
    }

    public class CameraService : ICameraService
    {
        public async Task<string?> TakePhotoAsync()
        {
            try
            {
                // Check camera permission
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        return null;
                    }
                }

                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take a photo"
                });

                return await ProcessPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error taking photo: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                return await ProcessPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking photo: {ex.Message}");
                return null;
            }
        }

        private async Task<string?> ProcessPhotoAsync(FileResult? photo)
        {
            if (photo == null)
                return null;

            // Save the file to local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
            {
                await stream.CopyToAsync(newStream);
            }

            return newFile;
        }
    }
}
