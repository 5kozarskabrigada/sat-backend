namespace SAT.API.Dtos;
 
 /// <summary>
 /// Request for login endpoint.
 /// </summary>

public class LoginRequest
{
    /// <summary>
    /// Phone or email used for login (primarily for admins in your flow).
    /// </summary>
    public string Identifier { get; set; } = null!;

    /// <summary>
    /// Password for login if you decide to protect admin with password.
    /// For student one-click registration you may ignore this.
    /// </summary>
    public string Password { get; set; } = null!;
}
