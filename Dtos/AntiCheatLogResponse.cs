namespace SAT.API.Dtos;

public class AntiCheatLogResponse
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public Guid StudentId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? DetailsJson { get; set; }
}
