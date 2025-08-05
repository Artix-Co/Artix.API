namespace Artix.API.Core.Domain.Entities.ValueObjects;

public class HistoricalDate
{
    public int Year { get; private set; } // Positive for AD, negative for BC
    public int Month { get; private set; }
    public int Day { get; private set; }

    public HistoricalDate(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public override string ToString() => Year < 0 ? $"{Math.Abs(Year)} BC" : $"{Year} AD";
}
