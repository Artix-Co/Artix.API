namespace Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;

public sealed class LatestOTPByPhoneNumberDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
}
