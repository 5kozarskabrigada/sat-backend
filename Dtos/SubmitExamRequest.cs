namespace SAT.API.Dtos;

public class SubmitExamRequest
{
    public Guid TestId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<SubmitAnswerDto> Responses { get; set; } = new();
}

public class SubmitAnswerDto
{
    public Guid QuestionId { get; set; }
    public string? Answer { get; set; }
}
