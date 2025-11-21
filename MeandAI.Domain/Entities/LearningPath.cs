namespace MeandAI.Domain.Entities;

public class LearningPath
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string TargetArea { get; private set; }
    public string? Description { get; private set; }

    private readonly List<LearningPathStep> _steps = new();
    public IReadOnlyCollection<LearningPathStep> Steps => _steps.AsReadOnly();

    private LearningPath() { }

    public LearningPath(string name, string targetArea, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        TargetArea = targetArea;
        Description = description;
    }

    public void Update(string name, string targetArea, string? description)
    {
        Name = name;
        TargetArea = targetArea;
        Description = description;
    }

    public LearningPathStep AddStep(string title, string? description, string? externalUrl, int order, int estimatedHours)
    {
        var step = new LearningPathStep(Id, title, description, externalUrl, order, estimatedHours);
        _steps.Add(step);
        _steps.Sort((a, b) => a.Order.CompareTo(b.Order));
        return step;
    }
}
