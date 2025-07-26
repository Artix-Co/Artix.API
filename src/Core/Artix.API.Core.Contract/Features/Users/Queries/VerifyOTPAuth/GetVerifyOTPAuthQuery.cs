namespace Artix.API.Core.Contract.Features.Users.Queries.VerifyOTPAuth;

using Primitives.Handlers;

public sealed class GetVerifyOTPAuthQuery : IQuery<VerifyOTPAuthDto>
{
    public required string PhoneNumber { get; set; }
    public required string OtpCode { get; set; }
}
