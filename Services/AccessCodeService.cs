using Microsoft.EntityFrameworkCore;
using SAT.API.Data;
using SAT.API.Models;

namespace SAT.API.Services;

public class AccessCodeService : IAccessCodeService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AccessCodeService> _logger;

    public AccessCodeService(AppDbContext db, ILogger<AccessCodeService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<AccessCode>> GenerateCodesAsync(Guid testId, int quantity, DateTime? expiresAt)
    {
        var testExists = await _db.Tests.AnyAsync(t => t.Id == testId);
        if (!testExists)
            throw new KeyNotFoundException("Test not found");

        var codes = new List<AccessCode>();

        for (var i = 0; i < quantity; i++)
        {
            var code = new AccessCode
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                Code = Guid.NewGuid().ToString(), // UUIDv4 style
                Status = AccessCodeStatus.INACTIVE,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };
            codes.Add(code);
        }

        _db.AccessCodes.AddRange(codes);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Generated {Count} access codes for test {TestId}", quantity, testId);
        return codes;
    }

    public async Task<List<AccessCode>> GetCodesForTestAsync(Guid testId)
    {
        return await _db.AccessCodes
            .Where(c => c.TestId == testId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> SetStatusAsync(string code, bool activate)
    {
        var entity = await _db.AccessCodes.FirstOrDefaultAsync(c => c.Code == code);
        if (entity == null)
            return false;

        if (activate)
        {
            if (entity.Status == AccessCodeStatus.ACTIVE)
                return true;

            entity.Status = AccessCodeStatus.ACTIVE;
            entity.ActivatedAt = DateTime.UtcNow;
        }
        else
        {
            if (entity.Status == AccessCodeStatus.INACTIVE)
                return true;

            entity.Status = AccessCodeStatus.INACTIVE;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Access code {Code} set status {Status}", code, entity.Status);
        return true;
    }

    public async Task<AccessCodeValidationResult> ValidateCodeAsync(string code)
    {
        var entity = await _db.AccessCodes.Include(c => c.Test).FirstOrDefaultAsync(c => c.Code == code);
        if (entity == null)
            return new AccessCodeValidationResult(false, "Code not found", null);

        if (entity.ExpiresAt.HasValue && entity.ExpiresAt.Value < DateTime.UtcNow)
            return new AccessCodeValidationResult(false, "Code expired", entity.TestId);

        if (entity.Status != AccessCodeStatus.ACTIVE)
            return new AccessCodeValidationResult(false, $"Code not active (status={entity.Status})", entity.TestId);

        if (entity.Test == null || !entity.Test.IsActive)
            return new AccessCodeValidationResult(false, "Test not active", entity.TestId);

        return new AccessCodeValidationResult(true, null, entity.TestId);
    }
}
