namespace MeandAI.Application.DTOs.Users;

public record UpdateUserProfileRequest(
    string Name,
    string CurrentRole,
    string DesiredArea
);
