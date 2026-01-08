// Dtos/ActiveStudentResponse.cs
namespace SAT.API.Dtos;

public class ActiveStudentResponse
{
    public Guid StudentId { get; set; }
    public string Name { get; set; } = default!;
    public int CurrentSectionIndex { get; set; }
    public int CurrentQuestionNumber { get; set; }
    public int TimeRemainingSeconds { get; set; }
    public int AnsweredCount { get; set; }
    public int TotalQuestions { get; set; }
    public int FlaggedCount { get; set; }
}
