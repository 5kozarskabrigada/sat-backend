using Microsoft.EntityFrameworkCore;
using SAT.API.Data;
using SAT.API.Models;

namespace SAT.API.Services;

public class TestService : ITestService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TestService> _logger;

    public TestService(AppDbContext db, ILogger<TestService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Test> CreateTestAsync(Guid adminId, string title)
    {
        var test = new Test
        {
            Id = Guid.NewGuid(),
            Title = title,
            CreatedBy = adminId,
            CreatedAt = DateTime.UtcNow,
            IsActive = false
        };

        _db.Tests.Add(test);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Test created {TestId} by admin {AdminId}", test.Id, adminId);
        return test;
    }

    public async Task<Test?> GetTestWithQuestionsAsync(Guid testId)
    {
        return await _db.Tests
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == testId);
    }

    public async Task<List<Test>> GetAllTestsAsync()
    {
        return await _db.Tests
            .Include(t => t.Questions)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> SetActiveAsync(Guid testId, bool isActive)
    {
        var test = await _db.Tests.FirstOrDefaultAsync(t => t.Id == testId);
        if (test == null)
            return false;

        test.IsActive = isActive;

        // Optional: manage StartedAt/EndedAt
        if (isActive && test.StartedAt == null)
            test.StartedAt = DateTime.UtcNow;
        if (!isActive && test.EndedAt == null)
            test.EndedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Test {TestId} set active={Active}", testId, isActive);
        return true;
    }

    public async Task<(Test test, List<Question> questions, List<TestSectionInfo> sections)?> GetTestForStudentByCodeAsync(
        Guid testId, string code, Guid studentAuthId)
    {
        // 1) Validate access code
        var accessCode = await _db.AccessCodes
            .Include(c => c.Test)
            .FirstOrDefaultAsync(c => c.TestId == testId && c.Code == code);

        if (accessCode == null)
        {
            _logger.LogWarning("Invalid access code {Code} for test {TestId}", code, testId);
            return null;
        }

        if (accessCode.Status != AccessCodeStatus.ACTIVE)
        {
            _logger.LogWarning("Access code {Code} not active. Status={Status}", code, accessCode.Status);
            return null;
        }

        // 2) Ensure test is active
        var test = accessCode.Test!;
        if (!test.IsActive)
        {
            _logger.LogWarning("Test {TestId} not active", test.Id);
            return null;
        }

        // 3) Load questions
        var questions = await _db.Questions
            .Where(q => q.TestId == test.Id)
            .OrderBy(q => q.SectionIndex)
            .ThenBy(q => q.QuestionNumber)
            .ToListAsync();

        // 4) Build sections definition (hard-coded SAT durations)
        var sections = new List<TestSectionInfo>
        {
            new TestSectionInfo(Index: 0, Name: "Reading & Writing", DurationSeconds: 65 * 60, SectionType: SectionType.READING),
            new TestSectionInfo(Index: 1, Name: "Math (No Calculator)", DurationSeconds: 25 * 60, SectionType: SectionType.MATH_NO_CALC),
            new TestSectionInfo(Index: 2, Name: "Math (With Calculator)", DurationSeconds: 55 * 60, SectionType: SectionType.MATH_CALC)
        };



        return (test, questions, sections);
    }
}
