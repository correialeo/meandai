namespace MeandAI.Application.DTOs.Skills;

public record UserSkillDto(
    Guid SkillId,
    string SkillName,
    string Category,
    int Level,
    DateTime UpdatedAt
);
