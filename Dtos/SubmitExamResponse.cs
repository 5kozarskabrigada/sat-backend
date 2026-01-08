namespace SAT.API.Dtos;

public class SubmitExamResponse
{
    public Guid TestId { get; set; }
    public Guid StudentId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
