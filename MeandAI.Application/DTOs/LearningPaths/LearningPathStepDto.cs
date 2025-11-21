namespace MeandAI.Application.DTOs.LearningPaths;

public record LearningPathStepDto(
    Guid Id,
    string Title,
    string? Description,
    string? ExternalCourseUrl,
    int Order,
    int EstimatedHours
);
