namespace Flowqueue_Backend.Analytics.Domain.Model.ValueObjects;

public sealed record AnalyticsPeriod
{
    public DateTimeOffset? From { get; }
    public DateTimeOffset? To { get; }

    public AnalyticsPeriod(DateTimeOffset? from, DateTimeOffset? to)
    {
        if (from is not null && to is not null && from > to)
            throw new ArgumentException("The start date cannot be greater than the end date.", nameof(from));

        From = from;
        To = to;
    }
}
