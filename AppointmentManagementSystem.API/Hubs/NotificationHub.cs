using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AppointmentManagementSystem.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // Kullanıcıyı kendi ID'sine göre bir gruba ekle
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                // Rol bazlı gruplara ekle
                if (!string.IsNullOrEmpty(userRole))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{userRole}");
                }
                
                Console.WriteLine($"User {userId} (Role: {userRole}) connected with connection ID: {Context.ConnectionId}");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                Console.WriteLine($"User {userId} disconnected");
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
