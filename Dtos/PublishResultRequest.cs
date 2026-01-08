// Dtos/PublishResultRequest.cs
namespace SAT.API.Dtos;

public class PublishResultRequest
{
    public Guid ResultId { get; set; }
    public bool Publish { get; set; }
}
