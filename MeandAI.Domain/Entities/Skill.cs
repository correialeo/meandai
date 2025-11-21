namespace MeandAI.Domain.Entities;

public class Skill
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Category { get; private set; }
    public string? Description { get; private set; }

    private Skill() { }

    public Skill(string name, string category, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Category = category;
        Description = description;
    }

    public void Update(string name, string category, string? description)
    {
        Name = name;
        Category = category;
        Description = description;
    }
}
