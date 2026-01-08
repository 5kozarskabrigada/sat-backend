namespace SAT.API.Models;

public enum UserRole
{
    STUDENT = 0,
    ADMIN = 1
}

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    /// <summary>
    /// STUDENT or ADMIN
    /// </summary>
    public UserRole Role { get; set; } = UserRole.STUDENT;

    /// <summary>
    /// Link to Supabase auth.users.id (UUID).
    /// </summary>
    public Guid? AuthId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<Test> TestsCreated { get; set; } = new List<Test>();

    public ICollection<StudentResponse> Responses { get; set; } = new List<StudentResponse>();

    public ICollection<Result> Results { get; set; } = new List<Result>();

    public ICollection<AntiCheatLog> AntiCheatLogs { get; set; } = new List<AntiCheatLog>();
}
