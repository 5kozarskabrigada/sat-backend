namespace SAT.API.Dtos;

public class TestSummaryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int QuestionCount { get; set; }
}
