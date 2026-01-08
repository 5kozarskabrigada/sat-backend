using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SAT.API.Models;

namespace SAT.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        // No db.Database.Migrate() here.

        if (!await db.Users.AnyAsync(cancellationToken))
        {
            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Phone = "0000000000",
                Email = "admin@example.com",
                Role = UserRole.ADMIN,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Seeding completed");
    }
}
