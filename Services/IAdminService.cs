// SAT.API/Services/IAdminService.cs
using SAT.API.Dtos;

public interface IAdminService
{
    Task<List<AdminResultSummaryResponse>> GetResultsForTestAsync(Guid testId);
    Task<bool> SetResultPublishedAsync(Guid resultId, bool publish);
    Task<List<ActiveStudentResponse>> GetActiveStudentsAsync(Guid testId);
    Task<List<AntiCheatLogResponse>> GetAntiCheatLogsAsync(Guid testId);

    Task<CreateStudentResponse> CreateStudentWithCredentialsAsync(string name, string phone);
}
