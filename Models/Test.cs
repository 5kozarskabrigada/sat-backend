namespace SAT.API.Models;

public class Test
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsActive { get; set; } = false;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public string? ConfigJson { get; set; }

    public User? Creator { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<AccessCode> AccessCodes { get; set; } = new List<AccessCode>();
    public ICollection<StudentResponse> StudentResponses { get; set; } = new List<StudentResponse>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
    public ICollection<AntiCheatLog> AntiCheatLogs { get; set; } = new List<AntiCheatLog>();
}
