// SAT.API/Services/AdminService.cs
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SAT.API.Data;
using SAT.API.Dtos;
using SAT.API.Models;

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
        if (result == null) return false;

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
                    Name = student.Name,
                    CurrentSectionIndex = 0,
                    CurrentQuestionNumber = 0,
                    TimeRemainingSeconds = 0,
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

    public async Task<CreateStudentResponse> CreateStudentWithCredentialsAsync(string name, string phone)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Phone = phone.Trim(),
            Email = null,
            Role = UserRole.STUDENT,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);

        var username = await GenerateUsernameAsync(name);
        var passwordPlain = GenerateRandomPassword(10);
        var passwordHash = HashPassword(passwordPlain);

        var creds = new LocalCredentials
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Username = username,
            PasswordHash = passwordHash,
        };

        _db.LocalCredentials.Add(creds);

        await _db.SaveChangesAsync();

        _logger.LogInformation("Created student {UserId} with username {Username}", user.Id, username);

        return new CreateStudentResponse
        {
            UserId = user.Id,
            Username = username,
            Password = passwordPlain
        };
    }

    private async Task<string> GenerateUsernameAsync(string name)
    {
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var first = parts.Length > 0 ? parts[0].ToLowerInvariant() : "user";
        var last = parts.Length > 1 ? parts[^1].ToLowerInvariant() : "student";

        var baseUsername = $"{first}.{last}";
        var username = baseUsername;
        var suffix = 0;

        while (await _db.LocalCredentials.AnyAsync(c => c.Username == username))
        {
            suffix++;
            username = $"{baseUsername}{suffix}";
        }

        return username;
    }

    private static string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%^&*";
        var data = new byte[length];
        RandomNumberGenerator.Fill(data);

        var sb = new StringBuilder(length);
        foreach (var b in data)
        {
            sb.Append(chars[b % chars.Length]);
        }

        return sb.ToString();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
