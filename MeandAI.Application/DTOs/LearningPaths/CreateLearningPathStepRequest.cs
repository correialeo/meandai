namespace MeandAI.Application.DTOs.LearningPaths;

public record CreateLearningPathStepRequest(
    string Title,
    string? Description,
    string? ExternalCourseUrl,
    int Order,
    int EstimatedHours
);
