namespace MeandAI.Application.DTOs.Users;

public record CreateUserRequest(
    string Name,
    string Email,
    string CurrentRole,
    string DesiredArea,
    string Password
);
