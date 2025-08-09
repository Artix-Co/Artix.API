namespace Artix.API.Infra.RabbitMQ.Interfaces;

using Models;

public interface IEmailService
{
    Task SendEmailAsync(NotificationMessage message);
}
