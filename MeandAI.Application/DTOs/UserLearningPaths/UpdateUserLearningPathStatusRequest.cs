using MeandAI.Domain.Enums;

namespace MeandAI.Application.DTOs.UserLearningPaths;

public record UpdateUserLearningPathStatusRequest(
    Guid UserLearningPathId,
    LearningPathStatus Status
);
