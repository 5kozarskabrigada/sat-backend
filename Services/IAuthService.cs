using SAT.API.Models;

namespace SAT.API.Services;

public interface IAuthService
{
    Task<(User user, string token)> RegisterStudentAsync(string name, string phone, string? email);
    Task<(User? user, string token)> LoginAsync(string identifier, string password);
    Task<User?> GetBySupabaseAuthIdAsync(Guid authId);
    Task<(User? user, string token)> LoginWithLocalCredentialsAsync(string username, string password);

}
