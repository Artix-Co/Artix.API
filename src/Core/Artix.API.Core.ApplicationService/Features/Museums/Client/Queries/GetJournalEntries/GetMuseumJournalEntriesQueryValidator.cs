using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetJournalEntries;

namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetJournalEntries;

public sealed class GetMuseumJournalEntriesQueryValidator : AbstractValidator<GetMuseumJournalEntriesQuery>
{
    public GetMuseumJournalEntriesQueryValidator() => RuleFor(x => x.MuseumId).GreaterThan(0);
}
