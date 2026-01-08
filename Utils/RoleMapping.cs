using SAT.API.Models;

namespace SAT.API.Utils;

public static class RoleMapping
{
    public static UserRole FromSupabaseRole(string? supabaseRole)
    {
        if (string.IsNullOrWhiteSpace(supabaseRole))
            return UserRole.STUDENT;

        return supabaseRole.ToUpperInvariant() switch
        {
            "ADMIN" => UserRole.ADMIN,
            // add other roles here only if they exist in your UserRole enum
            _ => UserRole.STUDENT
        };
    }

    public static string ToSupabaseRole(UserRole role)
    {
        return role switch
        {
            UserRole.ADMIN => "ADMIN",
            // map other enum values here if you add them later
            _ => "STUDENT"
        };
    }
}
