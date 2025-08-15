namespace Artix.API.Infra.RabbitMQ.Services.Notification;

using Artix.API.Infra.RabbitMQ.Interfaces.Notification;
using Artix.API.Infra.RabbitMQ.Models.Notification;

public class EmailService: IEmailService
{
    public Task SendEmailAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
