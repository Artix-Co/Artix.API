namespace Artix.API.Core.Domain.ValueObjects;

public class HistoricalDate
{
    public int Year { get; private set; } // Positive for AD, negative for BC
    public int Month { get; private set; }
    public int Day { get; private set; }

    public HistoricalDate(int year, int month, int day)
    {
        this.Year = year;
        this.Month = month;
        this.Day = day;
    }

    public override string ToString() => this.Year < 0 ? $"{Math.Abs(this.Year)} BC" : $"{this.Year} AD";
}
