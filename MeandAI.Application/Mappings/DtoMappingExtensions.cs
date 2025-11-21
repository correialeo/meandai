using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.DTOs.UserLearningPaths;
using MeandAI.Application.DTOs.Users;
using MeandAI.Domain.Entities;

namespace MeandAI.Application.Mappings;

public static class DtoMappingExtensions
{
    public static UserDto ToDto(this User user)
        => new(user.Id, user.Name, user.Email, user.CurrentRole, user.DesiredArea);

    public static SkillDto ToDto(this Skill skill)
        => new(skill.Id, skill.Name, skill.Category, skill.Description);

    public static LearningPathDto ToDto(this LearningPath path)
        => new(path.Id, path.Name, path.TargetArea, path.Description, path.Steps.Select(ToDto).ToList());

    public static LearningPathStepDto ToDto(this LearningPathStep step)
        => new(step.Id, step.Title, step.Description, step.ExternalCourseUrl, step.Order, step.EstimatedHours);

    public static UserLearningPathDto ToDto(this UserLearningPath userPath, string learningPathName)
        => new(userPath.Id, userPath.UserId, userPath.LearningPathId, learningPathName, userPath.Status, userPath.CreatedAt, userPath.CompletedAt);

    public static UserSkillDto ToDto(this UserSkill userSkill, Skill skill)
        => new(userSkill.SkillId, skill.Name, skill.Category, userSkill.Level, userSkill.UpdatedAt);
}
