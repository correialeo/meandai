namespace MeandAI.Application.DTOs.Skills;

public record CreateSkillRequest(
    string Name,
    string Category,
    string? Description
);
