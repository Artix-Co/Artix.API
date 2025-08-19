namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // شناسایی UserId از کلاینت (مثلاً از Claims یا QueryString)
        var userId = this.Context.User?.FindFirst("sub")?.Value ?? this.Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            // اضافه کردن کلاینت به گروه SignalR برای UserId
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // حذف کلاینت از گروه در صورت قطع اتصال
        var userId = this.Context.User?.FindFirst("sub")?.Value ?? this.Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
