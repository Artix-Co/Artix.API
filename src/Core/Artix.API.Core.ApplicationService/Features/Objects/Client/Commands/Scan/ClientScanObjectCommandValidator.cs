using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Client.Commands.Scan;

namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.Scan;

public sealed class ClientScanObjectCommandValidator : AbstractValidator<ClientScanObjectCommand>
{
    public ClientScanObjectCommandValidator()
    {
        RuleFor(x => x.ObjectId).NotEmpty();
        RuleFor(x => x.MuseumId).NotEmpty();
    }
}
