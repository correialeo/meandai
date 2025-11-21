namespace MeandAI.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string CurrentRole { get; private set; }
    public string DesiredArea { get; private set; }
    public string PasswordHash { get; private set; }

    private readonly List<UserSkill> _skills = new();
    public IReadOnlyCollection<UserSkill> Skills => _skills.AsReadOnly();

    private readonly List<UserLearningPath> _learningPaths = new();
    public IReadOnlyCollection<UserLearningPath> LearningPaths => _learningPaths.AsReadOnly();

    private User() 
    { 
        PasswordHash = string.Empty; // Inicializa para evitar warning
    }

    public User(string name, string email, string currentRole, string desiredArea, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        CurrentRole = currentRole ?? throw new ArgumentNullException(nameof(currentRole));
        DesiredArea = desiredArea ?? throw new ArgumentNullException(nameof(desiredArea));
        PasswordHash = passwordHash;
    }

    public void UpdateProfile(string name, string currentRole, string desiredArea)
    {
        Name = name;
        CurrentRole = currentRole;
        DesiredArea = desiredArea;
    }

    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            
        PasswordHash = passwordHash;
    }

    public void AddSkill(Skill skill, int level)
    {
        UserSkill? existing = _skills.FirstOrDefault(s => s.SkillId == skill.Id);
        if (existing is null)
        {
            _skills.Add(new UserSkill(Id, skill.Id, level));
        }
        else
        {
            existing.UpdateLevel(level);
        }
    }

    public UserLearningPath EnrollInLearningPath(LearningPath path)
    {
        if (_learningPaths.Any(lp => lp.LearningPathId == path.Id))
            return _learningPaths.First(lp => lp.LearningPathId == path.Id);

        var userPath = new UserLearningPath(Id, path.Id);
        _learningPaths.Add(userPath);
        return userPath;
    }
}
