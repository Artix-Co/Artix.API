namespace Artix.API.Infra.RabbitMQ.Services.Notification.Handlers;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class EmailNotificationHandler : INotificationHandler
{
    private readonly IEmailService _emailService;

    public EmailNotificationHandler(IEmailService emailService)
    {
        this._emailService = emailService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return this._emailService.SendEmailAsync(message);
    }
}
