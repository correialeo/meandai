namespace MeandAI.Application.DTOs.LearningPaths;

public record LearningPathDto(
    Guid Id,
    string Name,
    string TargetArea,
    string? Description,
    List<LearningPathStepDto> Steps
);
