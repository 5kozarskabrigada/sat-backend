namespace SAT.API.Services;

public interface ITimerService
{
    /// <summary>
    /// Validates that clientTimestamp is within allowed deviation from server time.
    /// Throws if invalid.
    /// </summary>
    void ValidateClientTimestamp(DateTime clientTimestamp);

    /// <summary>
    /// Checks if test still has time left for the student.
    /// </summary>
    Task<bool> HasTimeRemainingAsync(Guid testId, Guid studentId, DateTime serverNowUtc);
}
