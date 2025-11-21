namespace MeandAI.Application.DTOs.Users;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string CurrentRole,
    string DesiredArea
);
