using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT.API.Dtos;
using SAT.API.Models;
using SAT.API.Services;

namespace SAT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: /api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Create or fetch user (implementation inside AuthService)
        var (user, token) = await _authService.RegisterStudentAsync(
            request.Name.Trim(),
            request.Phone.Trim(),
            request.Email?.Trim()
        );

        var response = new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        };

        return Ok(response);
    }

    // POST: /api/auth/login  (primarily for admins, optional for students)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (user, token) = await _authService.LoginAsync(
            request.Identifier.Trim(),
            request.Password
        );

        if (user is null)
            return Unauthorized(new { message = "Invalid credentials" });

        var response = new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        };

        return Ok(response);
    }

    // GET: /api/auth/me
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me()
    {
        var sub = User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(sub))
            return Unauthorized(new { message = "Missing sub claim in token" });

        if (!Guid.TryParse(sub, out var authId))
            return Unauthorized(new { message = "Invalid sub claim format" });

        var user = await _authService.GetBySupabaseAuthIdAsync(authId);
        if (user is null)
            return NotFound(new { message = "User not found" });

        var dto = new MeResponse
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };

        return Ok(dto);
    }
}
