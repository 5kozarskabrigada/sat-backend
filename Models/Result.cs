namespace SAT.API.Models;

public class Result
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid StudentId { get; set; }

    public int TotalCorrect { get; set; }

    public int TotalIncorrect { get; set; }

    /// <summary>
    /// Raw score (total points) – simple count or scaled points.
    /// </summary>
    public int RawScore { get; set; }

    /// <summary>
    /// Percentage correct, 0–100.
    /// </summary>
    public double Percentage { get; set; }

    public bool IsPublished { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    // Navigation
    public Test? Test { get; set; }

    public User? Student { get; set; }
}
