namespace SAT.API.Dtos;

public class AutosaveRequest
{
    public Guid TestId { get; set; }
    public DateTime ClientTimestamp { get; set; }
    public List<AutosaveAnswerDto> Answers { get; set; } = new();
}

public class AutosaveAnswerDto
{
    public Guid QuestionId { get; set; }
    public string? SelectedAnswer { get; set; }
    public bool IsFlagged { get; set; }
}
