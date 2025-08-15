namespace Artix.API.Core.Domain.Entities.Museum;

using Exceptions;

public class ObjectHistoricalPeriod
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long HistoricalPeriodId { get; private set; }
    public virtual HistoricalPeriod HistoricalPeriod { get; private set; }

    protected ObjectHistoricalPeriod() { }

    private ObjectHistoricalPeriod(Object obj, HistoricalPeriod period)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (period == null)
            throw DomainException.InvalidValue(nameof(period));

        Object = obj;
        ObjectId = obj.Id;
        HistoricalPeriod = period;
        HistoricalPeriodId = period.Id;
    }

    public static ObjectHistoricalPeriod Create(Object obj, HistoricalPeriod period)
    {
        return new ObjectHistoricalPeriod(obj, period);
    }
}
