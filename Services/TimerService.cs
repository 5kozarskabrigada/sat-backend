using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAT.API.Data;

namespace SAT.API.Services;

public class TimerService : ITimerService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TimerService> _logger;
    private readonly int _maxDeviationSeconds;

    public TimerService(AppDbContext db, ILogger<TimerService> logger, IConfiguration config)
    {
        _db = db;
        _logger = logger;
        _maxDeviationSeconds = config.GetValue<int>("Exam:MaxTimerDeviationSeconds", 5);
    }

    public void ValidateClientTimestamp(DateTime clientTimestamp)
    {
        var serverNow = DateTime.UtcNow;
        var diff = Math.Abs((serverNow - clientTimestamp).TotalSeconds);
        if (diff > _maxDeviationSeconds)
        {
            _logger.LogWarning("Client timestamp deviation too high: {Diff}s", diff);
            throw new InvalidOperationException("Client time does not match server time");
        }
    }

    public async Task<bool> HasTimeRemainingAsync(Guid testId, Guid studentId, DateTime serverNowUtc)
    {
        // Simplified: just verify test is still active.
        var test = await _db.Tests.FirstOrDefaultAsync(t => t.Id == testId);
        if (test == null) return false;

        if (!test.IsActive)
            return false;

        // If you want per-student timer, you would store a start time per student/test.
        return true;
    }
}
