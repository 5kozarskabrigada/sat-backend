using Microsoft.EntityFrameworkCore;
using SAT.API.Data;
using SAT.API.Models;

namespace SAT.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly Supabase.Client _supabase;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, Supabase.Client supabase, ILogger<AuthService> logger)
    {
        _db = db;
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<(User user, string token)> RegisterStudentAsync(string name, string phone, string? email)
    {
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Phone == phone || (email != null && u.Email == email));

        if (existingUser != null)
        {
            _logger.LogInformation("User already exists, logging in: {Phone}", phone);
            var loginResult = await LoginInternalAsync(email ?? phone, GeneratePasswordFromPhone(phone), false);
            return (existingUser, loginResult.token);
        }

        var password = GeneratePasswordFromPhone(phone);

    var signUp = await _supabase.Auth.SignUp(email ?? $"{phone}@temp.local", password);

        if (signUp.User == null || string.IsNullOrEmpty(signUp.User.Id))
            throw new InvalidOperationException("Supabase signup failed, user is null");

        var authId = Guid.Parse(signUp.User.Id);

        var user = new User
        {
            Id = Guid.NewGuid(),
            AuthId = authId,
            Name = name,
            Phone = phone,
            Email = email,
            Role = UserRole.STUDENT,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (loggedInUser, token) = await LoginInternalAsync(email ?? phone, password, true);

        return (loggedInUser ?? user, token);
    }

    public async Task<(User? user, string token)> LoginAsync(string identifier, string password)
    {
        return await LoginInternalAsync(identifier, password, false);
    }

    public async Task<User?> GetBySupabaseAuthIdAsync(Guid authId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.AuthId == authId);
    }

   private async Task<(User? user, string token)> LoginInternalAsync(string identifier, string password, bool suppressErrors)
    {
        try
        {
            var signIn = await _supabase.Auth.SignIn(email: identifier, password: password);

            // Defensive null check
            if (signIn == null || signIn.User == null || string.IsNullOrEmpty(signIn.User.Id))
                throw new InvalidOperationException("Supabase login failed");

            // In Supabase 1.0 the access token is exposed directly on the result/session.
            // Adjust to whatever property Intellisense shows, for example AccessToken or Token.
            var token = signIn.AccessToken; // or signIn.Token, depending on the actual type

            var authId = Guid.Parse(signIn.User.Id);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.AuthId == authId);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    AuthId = authId,
                    Name = signIn.User.UserMetadata?.GetValueOrDefault("name")?.ToString() ?? identifier,
                    Phone = signIn.User.UserMetadata?.GetValueOrDefault("phone")?.ToString() ?? string.Empty,
                    Email = signIn.User.Email,
                    Role = UserRole.STUDENT,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return (user, token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Login failed for identifier {Identifier}", identifier);
            if (suppressErrors)
            {
                return (null, string.Empty);
            }

            throw;
        }
    }


    private static string GeneratePasswordFromPhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        var last4 = digits.Length >= 4 ? digits[^4..] : digits;
        return $"{last4}!Sat2024";
    }
}
