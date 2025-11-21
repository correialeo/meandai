namespace MeandAI.Application.DTOs.LearningPaths;

public record AddStepToPathRequest(
    string Title,
    string? Description,
    string? ExternalCourseUrl,
    int Order,
    int EstimatedHours
);
