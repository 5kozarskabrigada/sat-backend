namespace SAT.API.Models;

public class StudentResponse
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid StudentId { get; set; }

    public Guid QuestionId { get; set; }

    /// <summary>
    /// Selected answer value (e.g. "A", "B", "C", "42").
    /// </summary>
    public string? SelectedAnswer { get; set; }

    /// <summary>
    /// Whether student flagged for review in UI.
    /// </summary>
    public bool IsFlagged { get; set; } = false;

    /// <summary>
    /// Last autosave timestamp.
    /// </summary>
    public DateTime? AutosavedAt { get; set; }

    /// <summary>
    /// Final submit timestamp (null if never submitted).
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// Indicates if this record belongs to final submission vs autosave-only.
    /// </summary>
    public bool IsSubmitted { get; set; } = false;

    // Navigation
    public Test? Test { get; set; }

    public User? Student { get; set; }

    public Question? Question { get; set; }
}
