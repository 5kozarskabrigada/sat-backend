namespace SAT.API.Dtos;

/// <summary>
/// Response for /api/auth/me – current logged-in user profile.
/// </summary>
public class MeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
