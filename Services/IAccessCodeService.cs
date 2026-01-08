using SAT.API.Models;

namespace SAT.API.Services;

public record AccessCodeValidationResult(bool IsValid, string? Message, Guid? TestId);

public interface IAccessCodeService
{
    Task<List<AccessCode>> GenerateCodesAsync(Guid testId, int quantity, DateTime? expiresAt);

    Task<List<AccessCode>> GetCodesForTestAsync(Guid testId);

    Task<bool> SetStatusAsync(string code, bool activate);

    Task<AccessCodeValidationResult> ValidateCodeAsync(string code);
}
