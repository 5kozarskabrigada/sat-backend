using SAT.API.Models;

namespace SAT.API.Services;

public record TestSectionInfo(int Index, string Name, int DurationSeconds, SectionType SectionType);

public interface ITestService
{
    Task<Test> CreateTestAsync(Guid adminId, string title);
    Task<Test?> GetTestWithQuestionsAsync(Guid testId);
    Task<List<Test>> GetAllTestsAsync();
    Task<bool> SetActiveAsync(Guid testId, bool isActive);
    Task<(Test test, List<Question> questions, List<TestSectionInfo> sections)?> GetTestForStudentByCodeAsync(
        Guid testId, string code, Guid studentAuthId);
}
