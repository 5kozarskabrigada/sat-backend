// SAT.API/Models/LocalCredentials.cs
namespace SAT.API.Models;

public class LocalCredentials
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!; // hashed, not plain
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
