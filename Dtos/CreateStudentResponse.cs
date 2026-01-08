namespace SAT.API.Dtos;

public class CreateStudentResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!; // plain only returned once
}
