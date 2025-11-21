namespace MeandAI.Application.DTOs.Skills;

public record SkillDto(
    Guid Id,
    string Name,
    string Category,
    string? Description
);
