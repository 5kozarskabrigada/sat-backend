// Services/GradingService.cs
namespace SAT.API.Services;

public class GradingService : IGradingService
{
    public Task GradeTestAsync(Guid testId, Guid studentId)
    {
        // TODO: implement auto-grading
        return Task.CompletedTask;
    }
}
