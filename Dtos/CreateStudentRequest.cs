// SAT.API/Dtos/CreateStudentRequest.cs
namespace SAT.API.Dtos;

public class CreateStudentRequest
{
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
}

// SAT.API/Dtos/CreateStudentResponse.cs
namespace SAT.API.Dtos;

public class CreateStudentResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!; // plain only returned once
}
