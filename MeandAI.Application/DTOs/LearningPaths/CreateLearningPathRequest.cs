namespace MeandAI.Application.DTOs.LearningPaths;

public record CreateLearningPathRequest(
    string Name,
    string TargetArea,
    string? Description,
    List<CreateLearningPathStepRequest>? Steps
);
