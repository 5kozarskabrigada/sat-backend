namespace SAT.API.Models;

public enum AntiCheatEventType
{
    TAB_SWITCH = 0,
    FOCUS_LOSS = 1,
    KEYBOARD_BLOCK = 2,
    FULLSCREEN_EXIT = 3,
    TIMER_MISMATCH = 4
}

public class AntiCheatLog
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid StudentId { get; set; }

    public AntiCheatEventType EventType { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Extra JSON details (e.g. previous URL, key pressed, etc.).
    /// Stored as JSONB in PostgreSQL.
    /// </summary>
    public string? DetailsJson { get; set; }

    // Navigation
    public Test? Test { get; set; }

    public User? Student { get; set; }
}
