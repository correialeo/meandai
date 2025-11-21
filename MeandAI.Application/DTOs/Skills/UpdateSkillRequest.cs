namespace MeandAI.Application.DTOs.Skills;

public record UpdateSkillRequest(
    string Name,
    string Category,
    string? Description
);
