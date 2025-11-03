using Microsoft.Maui.Devices.Sensors;

namespace AppointmentManagementSystem.Maui.Services
{
    public interface ILocationService
    {
        Task<Location?> GetCurrentLocationAsync();
        Task<bool> RequestLocationPermissionAsync();
    }

    public class LocationService : ILocationService
    {
        public async Task<bool> RequestLocationPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error requesting location permission: {ex.Message}");
                return false;
            }
        }

        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                // Check permission first
                var hasPermission = await RequestLocationPermissionAsync();
                if (!hasPermission)
                    return null;

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

                return location;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting location: {ex.Message}");
                return null;
            }
        }
    }
}
