namespace Messaging.Contracts;

public class DayOfTheWeek
{
    public int DayNumber { get; init; }
    public string Name { get; init; } = string.Empty;
}


public class TimeOfDay
{
    public int Hour { get; init; }
    public int Minute { get; init; }
    public int Second { get; init; }
}