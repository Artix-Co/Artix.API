namespace Artix.API.Core.Contract.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

public sealed record ClientShuffledQuizzesDto(
    Guid Id,
    string Title,
    string Description,
    int XpReward,
    int BonusXp,
    int Tier,
    int Priority,
    DateTime? Deadline,
    bool IsSeasonal,
    IReadOnlyList<ShuffledQuizzesActionDto> RequiredActions);

public sealed record ShuffledQuizzesActionDto(
    string ActionType,
    string Details,
    int RequiredCount);
