namespace Artix.API.Core.ApplicationService.Features.Quizzes.Client.Queries.GetShuffledQuizzes;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Quizzes;
using Contract.Features.Quizzes.Client.Queries.GetShuffledQuizzes;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetShuffledQuizzesQueryHandler : QueryHandlerBase<GetClientShuffledQuizzesQuery, IEnumerable<ClientShuffledQuizzesDto>>
{
    private readonly IQuizQueryRepository _quizQueryRepository;


    public GetShuffledQuizzesQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IQuizQueryRepository quizQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._quizQueryRepository = quizQueryRepository;
    }

    public override async Task<Result<IEnumerable<ClientShuffledQuizzesDto>>> Handle(GetClientShuffledQuizzesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._quizQueryRepository.GetShuffledAsync(query, cancellationToken);
        return Result<IEnumerable<ClientShuffledQuizzesDto>>.Success(result);
    }
}
