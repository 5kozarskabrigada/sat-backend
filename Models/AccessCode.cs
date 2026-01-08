namespace SAT.API.Models;

public enum AccessCodeStatus
{
    INACTIVE = 0,
    ACTIVE = 1,
    USED = 2,
    EXPIRED = 3
}

public class AccessCode
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    /// <summary>
    /// UUID-style access code string shown to students.
    /// </summary>
    public string Code { get; set; } = null!;

    public AccessCodeStatus Status { get; set; } = AccessCodeStatus.INACTIVE;

    /// <summary>
    /// When assigned to a specific student (optional binding).
    /// </summary>
    public Guid? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ActivatedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    // Navigation
    public Test? Test { get; set; }

    public User? AssignedStudent { get; set; }
}
