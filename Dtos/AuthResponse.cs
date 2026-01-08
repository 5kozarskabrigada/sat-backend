namespace SAT.API.Dtos;
/// <summary>
/// Common response for login/register when you return a JWT to the client.
/// </summary>
public class AuthResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string Role { get; set; } = null!;   // "STUDENT" or "ADMIN"
    public string Token { get; set; } = null!;  // Supabase JWT or your own
}
