namespace Artix.API.Core.Contract.Features.Users.Client.Commands.InitiateOTPAuth;

using Primitives.Handlers;

public sealed record InitiateOTPAuthCommand(string PhoneNumber) : ICommand;
