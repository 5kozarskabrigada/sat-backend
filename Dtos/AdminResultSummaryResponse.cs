// Dtos/AdminResultSummaryResponse.cs
namespace SAT.API.Dtos;

public class AdminResultSummaryResponse
{
    public Guid ResultId { get; set; }
    public Guid TestId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = default!;
    public int TotalCorrect { get; set; }
    public int TotalIncorrect { get; set; }
    public double RawScore { get; set; }
    public double Percentage { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}
