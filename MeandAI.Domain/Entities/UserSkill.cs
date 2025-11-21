namespace MeandAI.Domain.Entities;

public class UserSkill
{
    public Guid UserId { get; private set; }
    public Guid SkillId { get; private set; }
    public int Level { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserSkill() { }

    public UserSkill(Guid userId, Guid skillId, int level)
    {
        UserId = userId;
        SkillId = skillId;
        Level = level;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLevel(int level)
    {
        Level = level;
        UpdatedAt = DateTime.UtcNow;
    }
}
