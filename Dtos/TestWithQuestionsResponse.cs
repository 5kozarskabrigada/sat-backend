namespace SAT.API.Dtos;

public class TestWithQuestionsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<TestSectionResponse> Sections { get; set; } = new();
    public List<QuestionResponse> Questions { get; set; } = new();
}

public class TestSectionResponse
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string SectionType { get; set; } = string.Empty;
}

public class QuestionResponse
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public int SectionIndex { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? PassageText { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public List<QuestionOptionResponse>? Options { get; set; }
}

public class QuestionOptionResponse
{
    public string Label { get; set; } = string.Empty;  // "A", "B", ...
    public string Text { get; set; } = string.Empty;
}
