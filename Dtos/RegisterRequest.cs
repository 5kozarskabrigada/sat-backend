// RegisterRequest.cs
public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}

// TestCreateRequest.cs
public class TestCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public List<SectionDto> Sections { get; set; } = new();
}

public class SectionDto
{
    public string Name { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string Type { get; set; } = string.Empty;
}

// SubmitResponseRequest.cs
public class SubmitResponseRequest
{
    public Guid TestId { get; set; }
    public List<ResponseAnswerDto> Responses { get; set; } = new();
    public DateTime SubmittedAt { get; set; }
}

public class ResponseAnswerDto
{
    public Guid QuestionId { get; set; }
    public string? Answer { get; set; }
    public bool IsFlagged { get; set; }
}
