namespace SAT.API.Dtos;

public class TestDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public List<QuestionResponse> Questions { get; set; } = new();
}