using MeandAI.Domain.Enums;

namespace MeandAI.Domain.Entities;

public class UserLearningPath
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid LearningPathId { get; private set; }
    public LearningPathStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private UserLearningPath() { }

    public UserLearningPath(Guid userId, Guid learningPathId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        LearningPathId = learningPathId;
        Status = LearningPathStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status == LearningPathStatus.NotStarted)
        {
            UpdateStatus(LearningPathStatus.InProgress);
        }
    }

    public void Complete()
    {
        UpdateStatus(LearningPathStatus.Completed);
    }

    public void UpdateStatus(LearningPathStatus status)
    {
        Status = status;
        CompletedAt = status == LearningPathStatus.Completed ? DateTime.UtcNow : null;
    }
}
