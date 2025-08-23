namespace Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;

public sealed record LatestOTPByPhoneNumberDto(Guid Id, string Code);
