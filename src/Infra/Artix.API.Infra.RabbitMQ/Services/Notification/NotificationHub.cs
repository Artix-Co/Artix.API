namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Microsoft.AspNetCore.SignalR;

public sealed class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = this.Context.User?.FindFirst("sub")?.Value ?? this.Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = this.Context.User?.FindFirst("sub")?.Value ?? this.Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
