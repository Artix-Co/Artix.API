namespace Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;

using Primitives.Handlers;

public sealed class GetLatestOTPByPhoneNumberQuery : IQuery<LatestOTPByPhoneNumberDto>
{
    public string PhoneNumber { get; set; }
    public string OtpCode { get; set; }
}
