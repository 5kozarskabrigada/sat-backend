using SAT.API.Dtos;
using SAT.API.Models;

namespace SAT.API.Services;

public interface IResponseService
{
    Task AutosaveAsync(Guid studentId, AutosaveRequest request);
    Task<SubmitExamResponse> SubmitExamAsync(Guid studentId, SubmitExamRequest request);
    Task SetFlagAsync(Guid studentId, Guid testId, Guid questionId, bool isFlagged);
    Task<List<StudentResponse>> GetStudentResponsesAsync(Guid testId, Guid studentId);
}
