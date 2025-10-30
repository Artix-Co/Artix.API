namespace Artix.API.Core.Contract.Features.Quizes.Queries.GetShuffledQuests;

public readonly record struct ShuffledQuestsDto(
    Guid BusinessId,
    string Title,
    ReadOnlyMemory<string> Options,
    byte CorrectOptionId,
    DateTime CreationDate);
