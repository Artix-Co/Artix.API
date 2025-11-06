namespace Artix.API.Core.Contract.Features.Users.Queries.VerifyOTPAuth;

using Primitives.Handlers;

public sealed record GetVerifyOTPAuthQuery(string PhoneNumber, string OtpCode) : IQuery<VerifyOTPAuthDto>;
public sealed record OtpSessionData(string Code, string Purpose, int Attempts);
