namespace SAT.API.Utils;

public static class TimeUtils
{
    public static DateTime UtcNow() => DateTime.UtcNow;

    public static int ToTotalSeconds(TimeSpan span) => (int)span.TotalSeconds;

    public static TimeSpan FromSeconds(int seconds) => TimeSpan.FromSeconds(seconds);

    public static bool IsPast(DateTime? utcTime)
    {
        return utcTime.HasValue && utcTime.Value <= DateTime.UtcNow;
    }

    public static bool IsBetween(DateTime nowUtc, DateTime? startUtc, DateTime? endUtc)
    {
        if (startUtc.HasValue && nowUtc < startUtc.Value) return false;
        if (endUtc.HasValue && nowUtc > endUtc.Value) return false;
        return true;
    }
}
