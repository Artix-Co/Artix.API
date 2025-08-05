namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;

public class HistoricalPeriod : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    private readonly List<ObjectHistoricalPeriod> _objectHistoricalPeriods = new();

    public virtual IReadOnlyCollection<ObjectHistoricalPeriod> ObjectHistoricalPeriods =>
        _objectHistoricalPeriods.AsReadOnly();

    protected HistoricalPeriod()
    {
    }

    private HistoricalPeriod(string name, string? description, DateTime? startDate, DateTime? endDate)
    {
        ValidateName(name);
        ValidateDates(startDate, endDate);

        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static HistoricalPeriod Create(string name, string? description = null, DateTime? startDate = null,
        DateTime? endDate = null)
    {
        return new HistoricalPeriod(name, description, startDate, endDate);
    }

    public void UpdateDetails(string name, string? description = null, DateTime? startDate = null,
        DateTime? endDate = null)
    {
        ValidateName(name);
        ValidateDates(startDate, endDate);

        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void AssignObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        if (_objectHistoricalPeriods.Any(ohp => ohp.ObjectId == obj.Id))
            return;

        var link = ObjectHistoricalPeriod.Create(obj,this);
        _objectHistoricalPeriods.Add(link);
    }

    public void RemoveObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        var link = _objectHistoricalPeriods.FirstOrDefault(ohp => ohp.ObjectId == obj.Id);
        if (link != null)
            _objectHistoricalPeriods.Remove(link);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
    }

    private void ValidateDates(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw DomainException.InvalidValue("StartDate cannot be later than EndDate.");
    }
}
