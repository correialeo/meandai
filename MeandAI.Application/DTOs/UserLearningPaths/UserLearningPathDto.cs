using MeandAI.Domain.Enums;

namespace MeandAI.Application.DTOs.UserLearningPaths;

public record UserLearningPathDto(
    Guid Id,
    Guid UserId,
    Guid LearningPathId,
    string LearningPathName,
    LearningPathStatus Status,
    DateTime CreatedAt,
    DateTime? CompletedAt
);
