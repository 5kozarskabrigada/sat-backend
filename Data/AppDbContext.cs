using Microsoft.EntityFrameworkCore;
using SAT.API.Models;

namespace SAT.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<StudentResponse> StudentResponses => Set<StudentResponse>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<AccessCode> AccessCodes => Set<AccessCode>();
    public DbSet<AntiCheatLog> AntiCheatLogs => Set<AntiCheatLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Phone).IsUnique();
            entity.HasIndex(u => u.Email);
            entity.Property(u => u.Role).HasConversion<string>();
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.ConfigJson).HasColumnType("jsonb");

            entity.HasOne(t => t.Creator)
                .WithMany(u => u.TestsCreated)
                .HasForeignKey(t => t.CreatedBy);

            entity.HasMany(t => t.Questions)
                .WithOne(q => q.Test)
                .HasForeignKey(q => q.TestId);

            entity.HasMany(t => t.AccessCodes)
                .WithOne(a => a.Test)
                .HasForeignKey(a => a.TestId);

            entity.HasMany(t => t.StudentResponses)
                .WithOne(r => r.Test)
                .HasForeignKey(r => r.TestId);

            entity.HasMany(t => t.Results)
                .WithOne(r => r.Test)
                .HasForeignKey(r => r.TestId);

            entity.HasMany(t => t.AntiCheatLogs)
                .WithOne(l => l.Test)
                .HasForeignKey(l => l.TestId);
        });

modelBuilder.Entity<Question>(entity =>
{
    entity.HasKey(q => q.Id);
    entity.Property(q => q.QuestionText).IsRequired();
    entity.Property(q => q.OptionsJson).HasColumnType("jsonb");
    entity.Property(q => q.QuestionType).HasConversion<string>();

    entity.HasIndex(q => new { q.TestId, q.SectionIndex, q.QuestionNumber })
        .IsUnique();
});


        modelBuilder.Entity<StudentResponse>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.Test)
                .WithMany(t => t.StudentResponses)
                .HasForeignKey(r => r.TestId);

            entity.HasOne(r => r.Student)
                .WithMany(u => u.Responses)
                .HasForeignKey(r => r.StudentId);

            entity.HasOne(r => r.Question)
                .WithMany(q => q.StudentResponses)
                .HasForeignKey(r => r.QuestionId);

            entity.HasIndex(r => new { r.TestId, r.StudentId, r.QuestionId })
                .IsUnique();
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.Test)
                .WithMany(t => t.Results)
                .HasForeignKey(r => r.TestId);

            entity.HasOne(r => r.Student)
                .WithMany(u => u.Results)
                .HasForeignKey(r => r.StudentId);
        });

        modelBuilder.Entity<AccessCode>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.Code).IsUnique();

            entity.HasOne(a => a.Test)
                .WithMany(t => t.AccessCodes)
                .HasForeignKey(a => a.TestId);
        });

modelBuilder.Entity<AntiCheatLog>(entity =>
{
    entity.HasKey(l => l.Id);
    entity.Property(l => l.EventType).HasConversion<string>();
    entity.Property(l => l.DetailsJson).HasColumnType("jsonb");

    entity.HasOne(l => l.Test)
        .WithMany(t => t.AntiCheatLogs)
        .HasForeignKey(l => l.TestId);

    entity.HasOne(l => l.Student)
        .WithMany(u => u.AntiCheatLogs)
        .HasForeignKey(l => l.StudentId);
});

    }
}
