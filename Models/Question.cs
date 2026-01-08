namespace SAT.API.Models;

public enum QuestionType
{
    MULTIPLE_CHOICE = 0,
    GRID_IN = 1
}

public enum SectionType
{
    READING = 0,
    MATH_NO_CALC = 1,
    MATH_CALC = 2
}

public class Question
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    /// <summary>
    /// Section index for ordering (0: R&W, 1: Math NC, 2: Math C).
    /// </summary>
    public int SectionIndex { get; set; }

    /// <summary>
    /// Question number within the test (1-based).
    /// </summary>
    public int QuestionNumber { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? PassageText { get; set; }

    public QuestionType QuestionType { get; set; } = QuestionType.MULTIPLE_CHOICE;

    /// <summary>
    /// JSON containing options: [{ "label": "A", "text": "..." }, ...]
    /// Stored as JSONB in PostgreSQL.
    /// </summary>
    public string? OptionsJson { get; set; }

    /// <summary>
    /// Correct answer key (e.g. "A", "B", "C", "D" or numeric for grid-in).
    /// </summary>
    public string CorrectAnswer { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Test? Test { get; set; }

    public ICollection<StudentResponse> StudentResponses { get; set; } = new List<StudentResponse>();
}
