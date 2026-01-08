namespace SAT.API.Dtos;

public class FlagQuestionRequest
{
    public Guid TestId { get; set; }
    public Guid QuestionId { get; set; }
    public bool IsFlagged { get; set; }
}
