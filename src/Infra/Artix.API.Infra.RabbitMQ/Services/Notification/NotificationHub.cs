namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

[Authorize]
public sealed class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;

        // log all claims for debugging
        _logger.LogDebug("SignalR connection attempt. ConnectionId={ConnectionId}, Claims={Claims}",
            connectionId,
            string.Join(", ", Context.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>())
        );

        var userId =
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized SignalR connection. ConnectionId={ConnectionId}", connectionId);
            throw new HubException("Unauthorized");
        }

        _logger.LogInformation("User {UserId} connected to NotificationHub. ConnectionId={ConnectionId}",
            userId, connectionId);

        await Groups.AddToGroupAsync(connectionId, userId);

        _logger.LogDebug("User {UserId} added to group {Group}. ConnectionId={ConnectionId}",
            userId, userId, connectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        var userId =
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning(
                "Unauthorized disconnect event. ConnectionId={ConnectionId}. Exception={Exception}",
                connectionId, exception?.Message);

            throw new HubException("Unauthorized");
        }

        _logger.LogInformation(
            "User {UserId} disconnected from NotificationHub. ConnectionId={ConnectionId}. Exception={Exception}",
            userId, connectionId, exception?.Message);

        await Groups.RemoveFromGroupAsync(connectionId, userId);

        _logger.LogDebug("User {UserId} removed from group {Group}. ConnectionId={ConnectionId}",
            userId, userId, connectionId);

        await base.OnDisconnectedAsync(exception);
    }
}
