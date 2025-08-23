namespace Artix.API.Core.Contract.Features.Users.Commands.InitiateOTPAuth;

using Primitives.Handlers;

public sealed record InitiateOTPAuthCommand(string PhoneNumber) : ICommand;
