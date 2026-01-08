using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT.API.Dtos;
using SAT.API.Services;

namespace SAT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("students")]
    public async Task<ActionResult<CreateStudentResponse>> CreateStudent([FromBody] CreateStudentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _adminService.CreateStudentWithCredentialsAsync(
            request.Name,
            request.Phone
        );

        // returns username + password ONCE so admin can give it to the student
        return Ok(result);
    }

    // GET: /api/admin/results/{testId}
    [HttpGet("results/{testId:guid}")]
    public async Task<ActionResult<List<AdminResultSummaryResponse>>> GetResults(Guid testId)
    {
        var results = await _adminService.GetResultsForTestAsync(testId);
        return Ok(results);
    }

    // POST: /api/admin/results/publish
    [HttpPost("results/publish")]
    public async Task<IActionResult> Publish([FromBody] PublishResultRequest request)
    {
        var success = await _adminService.SetResultPublishedAsync(request.ResultId, request.Publish);
        if (!success)
            return NotFound(new { message = "Result not found" });

        return NoContent();
    }

    // GET: /api/admin/active-students/{testId}
    [HttpGet("active-students/{testId:guid}")]
    public async Task<IActionResult> GetActiveStudents(Guid testId)
    {
        var students = await _adminService.GetActiveStudentsAsync(testId);
        return Ok(students);
    }

    // GET: /api/admin/anticheat/{testId}
    [HttpGet("anticheat/{testId:guid}")]
    public async Task<IActionResult> GetAntiCheatLogs(Guid testId)
    {
        var logs = await _adminService.GetAntiCheatLogsAsync(testId);
        return Ok(logs);
    }
}
