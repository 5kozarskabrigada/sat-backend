// Services/IAdminService.cs
using SAT.API.Dtos;

namespace SAT.API.Services;

public interface IAdminService
{
    Task<List<AdminResultSummaryResponse>> GetResultsForTestAsync(Guid testId);
    Task<bool> SetResultPublishedAsync(Guid resultId, bool publish);
    Task<List<ActiveStudentResponse>> GetActiveStudentsAsync(Guid testId);
    Task<List<AntiCheatLogResponse>> GetAntiCheatLogsAsync(Guid testId);
}
