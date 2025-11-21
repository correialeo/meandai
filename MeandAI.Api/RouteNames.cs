namespace MeandAI.Api;

/// <summary>
/// Centralizes the route names used by the API to create HATEOAS links in a strongly typed manner.
/// </summary>
public static class RouteNames
{
    public static class Users
    {
        public const string Create = nameof(Users) + "_Create";
        public const string GetById = nameof(Users) + "_GetById";
        public const string GetPaged = nameof(Users) + "_GetPaged";
        public const string Update = nameof(Users) + "_Update";
        public const string Delete = nameof(Users) + "_Delete";
        public const string AddSkill = nameof(Users) + "_AddSkill";
        public const string GetSkills = nameof(Users) + "_GetSkills";
    }

    public static class Skills
    {
        public const string Create = nameof(Skills) + "_Create";
        public const string GetAll = nameof(Skills) + "_GetAll";
        public const string GetById = nameof(Skills) + "_GetById";
        public const string GetByCategory = nameof(Skills) + "_GetByCategory";
        public const string Update = nameof(Skills) + "_Update";
        public const string Delete = nameof(Skills) + "_Delete";
    }

    public static class LearningPaths
    {
        public const string Create = nameof(LearningPaths) + "_Create";
        public const string GetAll = nameof(LearningPaths) + "_GetAll";
        public const string GetById = nameof(LearningPaths) + "_GetById";
        public const string GetByTargetArea = nameof(LearningPaths) + "_GetByTargetArea";
        public const string Update = nameof(LearningPaths) + "_Update";
        public const string Delete = nameof(LearningPaths) + "_Delete";
        public const string AddStep = nameof(LearningPaths) + "_AddStep";
    }

    public static class UserLearningPaths
    {
        public const string Enroll = nameof(UserLearningPaths) + "_Enroll";
        public const string GetById = nameof(UserLearningPaths) + "_GetById";
        public const string GetByUser = nameof(UserLearningPaths) + "_GetByUser";
        public const string UpdateStatus = nameof(UserLearningPaths) + "_UpdateStatus";
    }
}
