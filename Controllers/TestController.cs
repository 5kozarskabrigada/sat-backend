using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT.API.Dtos;
using SAT.API.Services;

namespace SAT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private readonly ITestService _testService;

    public TestsController(ITestService testService)
    {
        _testService = testService;
    }

    // POST: /api/tests/create   [ADMIN]
    [HttpPost("create")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<TestDetailResponse>> Create([FromBody] TestCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(adminIdString) || !Guid.TryParse(adminIdString, out var adminId))
            return Unauthorized(new { message = "Invalid admin identifier" });

        var test = await _testService.CreateTestAsync(adminId, request.Title.Trim());

        var dto = new TestDetailResponse
        {
            Id = test.Id,
            Title = test.Title,
            IsActive = test.IsActive,
            CreatedAt = test.CreatedAt,
            StartedAt = test.StartedAt,
            EndedAt = test.EndedAt,
            Questions = new List<QuestionResponse>() // questions added in GetById
        };

        return CreatedAtAction(nameof(GetById), new { id = test.Id }, dto);
    }

    // GET: /api/tests/{id}   [ADMIN] view full test
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<TestDetailResponse>> GetById(Guid id)
    {
        var test = await _testService.GetTestWithQuestionsAsync(id);
        if (test is null)
            return NotFound();

        var dto = new TestDetailResponse
        {
            Id = test.Id,
            Title = test.Title,
            IsActive = test.IsActive,
            CreatedAt = test.CreatedAt,
            StartedAt = test.StartedAt,
            EndedAt = test.EndedAt,
            Questions = test.Questions
                .OrderBy(q => q.SectionIndex)
                .ThenBy(q => q.QuestionNumber)
                .Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    TestId = q.TestId,
                    SectionIndex = q.SectionIndex,
                    QuestionNumber = q.QuestionNumber,
                    QuestionText = q.QuestionText,
                    PassageText = q.PassageText,
                    QuestionType = q.QuestionType.ToString(),
                    Options = q.OptionsJson is null
                        ? null
                        : System.Text.Json.JsonSerializer
                            .Deserialize<List<QuestionOptionResponse>>(q.OptionsJson)
                })
                .ToList()
        };

        return Ok(dto);
    }

    // GET: /api/tests  [ADMIN] list tests
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<List<TestSummaryResponse>>> GetAll()
    {
        var tests = await _testService.GetAllTestsAsync();

        var dtos = tests.Select(t => new TestSummaryResponse
        {
            Id = t.Id,
            Title = t.Title,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            QuestionCount = t.Questions.Count
        }).ToList();

        return Ok(dtos);
    }

    // POST: /api/tests/{id}/activate  [ADMIN]
    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var success = await _testService.SetActiveAsync(id, true);
        if (!success)
            return NotFound(new { message = "Test not found" });

        return NoContent();
    }

    // POST: /api/tests/{id}/deactivate  [ADMIN]
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var success = await _testService.SetActiveAsync(id, false);
        if (!success)
            return NotFound(new { message = "Test not found" });

        return NoContent();
    }

    // GET: /api/tests/{testId}/code/{code}
    // Student uses this to start exam via access code.
    [HttpGet("{testId:guid}/code/{code}")]
    [Authorize] // any authenticated student
    public async Task<ActionResult<TestWithQuestionsResponse>> GetByAccessCode(Guid testId, string code)
    {
        var studentAuthId = User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(studentAuthId) || !Guid.TryParse(studentAuthId, out var authGuid))
            return Unauthorized(new { message = "Invalid token" });

        // Service should validate:
        // - code belongs to this test
        // - code is ACTIVE / not used
        // - user is allowed to use it
        var result = await _testService.GetTestForStudentByCodeAsync(testId, code, authGuid);
        if (result is null)
            return NotFound(new { message = "Invalid or inactive access code" });

        var (test, questions, sections) = result.Value;

        var dto = new TestWithQuestionsResponse
        {
            Id = test.Id,
            Title = test.Title,
            Sections = sections.Select(s => new TestSectionResponse
            {
                Index = s.Index,
                Name = s.Name,
                DurationSeconds = s.DurationSeconds,
                SectionType = s.SectionType.ToString()
            }).ToList(),
            Questions = questions
                .OrderBy(q => q.SectionIndex)
                .ThenBy(q => q.QuestionNumber)
                .Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    TestId = q.TestId,
                    SectionIndex = q.SectionIndex,
                    QuestionNumber = q.QuestionNumber,
                    QuestionText = q.QuestionText,
                    PassageText = q.PassageText,
                    QuestionType = q.QuestionType.ToString(),
                    Options = q.OptionsJson is null
                        ? null
                        : System.Text.Json.JsonSerializer
                            .Deserialize<List<QuestionOptionResponse>>(q.OptionsJson)
                })
                .ToList()
        };

        return Ok(dto);
    }
}
