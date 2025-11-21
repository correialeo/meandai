namespace MeandAI.Application.DTOs.Skills;

public record AddUserSkillRequest(
    Guid SkillId,
    int Level
);
