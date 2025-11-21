namespace MeandAI.Application.DTOs.LearningPaths;

public record UpdateLearningPathRequest(
    string Name,
    string TargetArea,
    string? Description
);
