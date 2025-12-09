namespace Artix.API.Core.ApplicationService.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

using Primitives;
using Artix.API.Core.Contract.Features.Quizzes.Queries;
using Artix.API.Core.Contract.Features.Quizzes.Queries.GetShuffledQuizzes;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetShuffledQuizzesQueryHandler : QueryHandlerBase<GetShuffledQuizzesQuery, IEnumerable<ShuffledQuizzesDto>>
{
    private readonly IQuizQueryRepository _quizQueryRepository;


    public GetShuffledQuizzesQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IQuizQueryRepository quizQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._quizQueryRepository = quizQueryRepository;
    }

    public override async Task<Result<IEnumerable<ShuffledQuizzesDto>>> Handle(GetShuffledQuizzesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._quizQueryRepository.GetShuffledAsync(query, cancellationToken);
        return Result<IEnumerable<ShuffledQuizzesDto>>.Success(result);
    }
}
