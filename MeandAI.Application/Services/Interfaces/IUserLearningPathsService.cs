using MeandAI.Application.DTOs.UserLearningPaths;

namespace MeandAI.Application.Services.Interfaces;

public interface IUserLearningPathsService
{
    Task<UserLearningPathDto> EnrollAsync(Guid userId, EnrollUserInPathRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UserLearningPathDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserLearningPathDto?> UpdateStatusAsync(Guid userId, UpdateUserLearningPathStatusRequest request, CancellationToken cancellationToken = default);
}
