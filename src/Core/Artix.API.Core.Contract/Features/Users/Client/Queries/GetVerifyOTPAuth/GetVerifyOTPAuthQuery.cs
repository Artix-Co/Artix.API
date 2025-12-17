namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;

using Domain.Entities.OTP.Enums;
using Primitives.Handlers;

public sealed record GetVerifyOTPAuthQuery(string PhoneNumber, string OtpCode) : IQuery<VerifyOTPAuthDto>;
public sealed record OtpSessionData(string Code, PurposeType Purpose, int Attempts);
