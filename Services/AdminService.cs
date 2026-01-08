using SAT.API.Dtos;
using SAT.API.Models;
using SAT.API.Data;
using Microsoft.EntityFrameworkCore;

namespace SAT.API.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AdminService> _logger;

    public AdminService(AppDbContext db, ILogger<AdminService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<AdminResultSummaryResponse>> GetResultsForTestAsync(Guid testId)
    {
        var results = await _db.Results
            .Include(r => r.Student)
            .Where(r => r.TestId == testId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return results.Select(r => new AdminResultSummaryResponse
        {
            ResultId = r.Id,
            TestId = r.TestId,
            StudentId = r.StudentId,
            StudentName = r.Student?.Name ?? "Unknown",
            TotalCorrect = r.TotalCorrect,
            TotalIncorrect = r.TotalIncorrect,
            RawScore = r.RawScore,
            Percentage = r.Percentage,
            IsPublished = r.IsPublished,
            CreatedAt = r.CreatedAt,
            PublishedAt = r.PublishedAt
        }).ToList();
    }

    public async Task<bool> SetResultPublishedAsync(Guid resultId, bool publish)
    {
        var result = await _db.Results.FirstOrDefaultAsync(r => r.Id == resultId);
        if (result == null)
            return false;

        result.IsPublished = publish;
        result.PublishedAt = publish ? DateTime.UtcNow : null;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Result {ResultId} publish={Publish}", resultId, publish);
        return true;
    }

public async Task<List<ActiveStudentResponse>> GetActiveStudentsAsync(Guid testId)
{
    var responses = await _db.StudentResponses
        .Include(r => r.Student)
        .Where(r => r.TestId == testId)
        .ToListAsync();

    var grouped = responses
        .GroupBy(r => r.StudentId)
        .Select(g =>
        {
            var first = g.FirstOrDefault();
            if (first == null || first.Student == null)
            {
                return new ActiveStudentResponse
                {
                    StudentId = g.Key,
                    Name = "Unknown",
                    CurrentSectionIndex = 0,
                    CurrentQuestionNumber = 0,
                    TimeRemainingSeconds = 0,
                    AnsweredCount = 0,
                    TotalQuestions = 0,
                    FlaggedCount = 0
                };
            }

            var student = first.Student;
            var answered = g.Count(r => !string.IsNullOrEmpty(r.SelectedAnswer));
            var flagged = g.Count(r => r.IsFlagged);
            var totalQuestions = g.Count();

            return new ActiveStudentResponse
            {
                StudentId = student.Id,
                Name = student.Name, // or FullName depending on your User model
                CurrentSectionIndex = 0,          // TODO: fill from your model if you track it
                CurrentQuestionNumber = 0,        // TODO
                TimeRemainingSeconds = 0,         // TODO
                AnsweredCount = answered,
                TotalQuestions = totalQuestions,
                FlaggedCount = flagged
            };
        })
        .ToList();

    return grouped;
}


public async Task<List<AntiCheatLogResponse>> GetAntiCheatLogsAsync(Guid testId)
{
    var logs = await _db.AntiCheatLogs
        .Where(l => l.TestId == testId)
        .OrderBy(l => l.Timestamp)
        .ToListAsync();

    return logs.Select(l => new AntiCheatLogResponse
    {
        Id = l.Id,
        TestId = l.TestId,
        StudentId = l.StudentId,
        EventType = l.EventType.ToString(),
        Timestamp = l.Timestamp,
        DetailsJson = l.DetailsJson
    }).ToList();
}

}
