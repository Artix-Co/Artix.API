namespace Artix.API.Infra.RabbitMQ.Services.Handlers;

using Interfaces;
using Models;

public class EmailNotificationHandler : INotificationHandler
{
    private readonly IEmailService _emailService;

    public EmailNotificationHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task HandleAsync(NotificationMessage message)
    {
        return _emailService.SendEmailAsync(message);
    }
}
