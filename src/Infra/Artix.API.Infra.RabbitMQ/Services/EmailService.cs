namespace Artix.API.Infra.RabbitMQ.Services;

using Interfaces;
using Models;

public class EmailService: IEmailService
{
    public Task SendEmailAsync(NotificationMessage message)
    {
        throw new NotImplementedException();
    }
}
