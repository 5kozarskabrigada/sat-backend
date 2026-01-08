using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT.API.Dtos;
using SAT.API.Services;

namespace SAT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // all endpoints require logged-in student
public class ResponsesController : ControllerBase
{
    private readonly IResponseService _responseService;

    public ResponsesController(IResponseService responseService)
    {
        _responseService = responseService;
    }

    private Guid GetStudentIdOrThrow()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
            throw new UnauthorizedAccessException("Invalid student identifier in token");
        return guid;
    }

    // POST: /api/responses/autosave
    [HttpPost("autosave")]
    public async Task<IActionResult> Autosave([FromBody] AutosaveRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var studentId = GetStudentIdOrThrow();

        await _responseService.AutosaveAsync(studentId, request);

        return NoContent();
    }

    // POST: /api/responses/submit
    [HttpPost("submit")]
    public async Task<ActionResult<SubmitExamResponse>> Submit([FromBody] SubmitExamRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var studentId = GetStudentIdOrThrow();

        var result = await _responseService.SubmitExamAsync(studentId, request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // POST: /api/responses/flag
    [HttpPost("flag")]
    public async Task<IActionResult> Flag([FromBody] FlagQuestionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var studentId = GetStudentIdOrThrow();

        await _responseService.SetFlagAsync(studentId, request.TestId, request.QuestionId, request.IsFlagged);

        return NoContent();
    }

    // GET: /api/responses/{testId}/student/{id}  [ADMIN] – view single student's answers
    [HttpGet("{testId:guid}/student/{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetStudentResponses(Guid testId, Guid id)
    {
        var responses = await _responseService.GetStudentResponsesAsync(testId, id);
        return Ok(responses);
    }
}
