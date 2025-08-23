namespace Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;

using Primitives.Handlers;

public sealed record GetLatestOTPByPhoneNumberQuery(string PhoneNumber, string OtpCode)
    : IQuery<LatestOTPByPhoneNumberDto>;
