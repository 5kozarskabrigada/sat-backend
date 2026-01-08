// Services/IGradingService.cs
namespace SAT.API.Services;

public interface IGradingService
{
    Task GradeTestAsync(Guid testId, Guid studentId);
}
