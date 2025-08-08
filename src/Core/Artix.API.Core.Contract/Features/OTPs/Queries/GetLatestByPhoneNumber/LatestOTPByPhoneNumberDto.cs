namespace Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;

public sealed class LatestOTPByPhoneNumberDto
{
    public long Id { get; set; }
    public string Code { get; set; }
}
