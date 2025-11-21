namespace MeandAI.Domain.Entities;

public class LearningPathStep
{
    public Guid Id { get; private set; }
    public Guid LearningPathId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string? ExternalCourseUrl { get; private set; }
    public int Order { get; private set; }
    public int EstimatedHours { get; private set; }

    private LearningPathStep() { }

    public LearningPathStep(Guid learningPathId, string title, string? description, string? externalCourseUrl, int order, int estimatedHours)
    {
        Id = Guid.NewGuid();
        LearningPathId = learningPathId;
        Title = title;
        Description = description;
        ExternalCourseUrl = externalCourseUrl;
        Order = order;
        EstimatedHours = estimatedHours;
    }

    public void Update(string title, string? description, string? externalCourseUrl, int order, int estimatedHours)
    {
        Title = title;
        Description = description;
        ExternalCourseUrl = externalCourseUrl;
        Order = order;
        EstimatedHours = estimatedHours;
    }
}
