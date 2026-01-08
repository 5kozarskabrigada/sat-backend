using Microsoft.EntityFrameworkCore;
using SAT.API.Data;
using SAT.API.Dtos;
using SAT.API.Models;

namespace SAT.API.Services;

public class ResponseService : IResponseService
{
    private readonly AppDbContext _db;
    private readonly ITimerService _timer;
    private readonly ILogger<ResponseService> _logger;

    public ResponseService(AppDbContext db, ITimerService timer, ILogger<ResponseService> logger)
    {
        _db = db;
        _timer = timer;
        _logger = logger;
    }

    public async Task AutosaveAsync(Guid studentId, AutosaveRequest request)
    {
        // Validate timestamp to prevent manipulation
        _timer.ValidateClientTimestamp(request.ClientTimestamp);

        var serverNow = DateTime.UtcNow;
        var hasTime = await _timer.HasTimeRemainingAsync(request.TestId, studentId, serverNow);
        if (!hasTime)
            throw new InvalidOperationException("Time is over for this test");

        var questionIds = request.Answers.Select(a => a.QuestionId).Distinct().ToList();
        var existing = await _db.StudentResponses
            .Where(r => r.TestId == request.TestId && r.StudentId == studentId && questionIds.Contains(r.QuestionId))
            .ToListAsync();

        foreach (var item in request.Answers)
        {
            var response = existing.FirstOrDefault(r => r.QuestionId == item.QuestionId);
            if (response == null)
            {
                response = new StudentResponse
                {
                    Id = Guid.NewGuid(),
                    TestId = request.TestId,
                    StudentId = studentId,
                    QuestionId = item.QuestionId
                };
                _db.StudentResponses.Add(response);
            }

            response.SelectedAnswer = item.SelectedAnswer;
            response.IsFlagged = item.IsFlagged;
            response.AutosavedAt = serverNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<SubmitExamResponse> SubmitExamAsync(Guid studentId, SubmitExamRequest request)
    {
        _timer.ValidateClientTimestamp(request.SubmittedAt);

        var serverNow = DateTime.UtcNow;
        var hasTime = await _timer.HasTimeRemainingAsync(request.TestId, studentId, serverNow);
        if (!hasTime)
        {
            return new SubmitExamResponse
            {
                TestId = request.TestId,
                StudentId = studentId,
                Success = false,
                Message = "Time is over for this test"
            };
        }

        // Load all questions for grading
        var questions = await _db.Questions
            .Where(q => q.TestId == request.TestId)
            .ToDictionaryAsync(q => q.Id, q => q);

        // Load existing responses to update as submitted
        var existing = await _db.StudentResponses
            .Where(r => r.TestId == request.TestId && r.StudentId == studentId)
            .ToListAsync();

        foreach (var item in request.Responses)
        {
            if (!questions.ContainsKey(item.QuestionId))
                continue;

            var response = existing.FirstOrDefault(r => r.QuestionId == item.QuestionId);
            if (response == null)
            {
                response = new StudentResponse
                {
                    Id = Guid.NewGuid(),
                    TestId = request.TestId,
                    StudentId = studentId,
                    QuestionId = item.QuestionId
                };
                _db.StudentResponses.Add(response);
                existing.Add(response);
            }

            response.SelectedAnswer = item.Answer;
            response.SubmittedAt = serverNow;
            response.IsSubmitted = true;
        }

        await _db.SaveChangesAsync();

        // Grade
        var totalQuestions = questions.Count;
        var totalCorrect = 0;

        foreach (var kv in questions)
        {
            var qId = kv.Key;
            var question = kv.Value;

            var response = existing.FirstOrDefault(r => r.QuestionId == qId);
            if (response != null &&
                !string.IsNullOrWhiteSpace(response.SelectedAnswer) &&
                string.Equals(response.SelectedAnswer.Trim(), question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                totalCorrect++;
            }
        }

        var totalIncorrect = totalQuestions - totalCorrect;
        var percentage = totalQuestions == 0 ? 0 : (double)totalCorrect / totalQuestions * 100;

        // Create or update result
        var result = await _db.Results.FirstOrDefaultAsync(r => r.TestId == request.TestId && r.StudentId == studentId);
        if (result == null)
        {
            result = new Result
            {
                Id = Guid.NewGuid(),
                TestId = request.TestId,
                StudentId = studentId,
                CreatedAt = serverNow
            };
            _db.Results.Add(result);
        }

        result.TotalCorrect = totalCorrect;
        result.TotalIncorrect = totalIncorrect;
        result.RawScore = totalCorrect;
        result.Percentage = Math.Round(percentage, 2);

        await _db.SaveChangesAsync();

        _logger.LogInformation("Exam submitted. Test={TestId} Student={StudentId} Correct={Correct}/{Total}",
            request.TestId, studentId, totalCorrect, totalQuestions);

        return new SubmitExamResponse
        {
            TestId = request.TestId,
            StudentId = studentId,
            Success = true,
            Message = "Exam submitted successfully"
        };
    }

    public async Task SetFlagAsync(Guid studentId, Guid testId, Guid questionId, bool isFlagged)
    {
        var response = await _db.StudentResponses
            .FirstOrDefaultAsync(r => r.TestId == testId && r.StudentId == studentId && r.QuestionId == questionId);

        if (response == null)
        {
            response = new StudentResponse
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                StudentId = studentId,
                QuestionId = questionId,
                IsFlagged = isFlagged,
                AutosavedAt = DateTime.UtcNow
            };
            _db.StudentResponses.Add(response);
        }
        else
        {
            response.IsFlagged = isFlagged;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<List<StudentResponse>> GetStudentResponsesAsync(Guid testId, Guid studentId)
    {
        return await _db.StudentResponses
            .Where(r => r.TestId == testId && r.StudentId == studentId)
            .OrderBy(r => r.QuestionId)
            .ToListAsync();
    }
}
