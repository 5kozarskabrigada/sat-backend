// Dtos/StartExamRequest.cs
namespace SAT.API.Dtos;

public class StartExamRequest
{
    public Guid TestId { get; set; }
    public DateTime ClientStartTimeUtc { get; set; }
}
