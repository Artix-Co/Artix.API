namespace Artix.API.Core.Domain.Entities.Season;

using Common;
using Exceptions;
using User;

public sealed class Season : BaseAggregateRoot
{
    public string? Name { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool? IsActive { get; private set; }

    private readonly List<SeasonTask> _seasonTasks = new();
    public IReadOnlyCollection<SeasonTask> SeasonTasks => _seasonTasks.AsReadOnly();

    private readonly List<UserSeasonProgress> _userSeasonProgresses = new();
    public IReadOnlyCollection<UserSeasonProgress> UserSeasonProgresses => _userSeasonProgresses.AsReadOnly();

    public void UpdateDetails(string? name, DateOnly? startDate, DateOnly? endDate, bool? isActive)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
        SetModified();
    }

    public void AddSeasonTask(SeasonTask task)
    {
        if (task == null)
            throw DomainException.InvalidValue(nameof(task));
        if (!_seasonTasks.Contains(task))
        {
            _seasonTasks.Add(task);
            AddEntity(task);
        }
    }

    public void RemoveSeasonTask(SeasonTask task)
    {
        if (task is null)
            throw DomainException.InvalidValue(nameof(task));

        if (_seasonTasks.Remove(task))
            RemoveEntity(task);
    }
}
