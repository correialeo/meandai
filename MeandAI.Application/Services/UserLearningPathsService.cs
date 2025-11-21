using MeandAI.Application.DTOs.UserLearningPaths;
using MeandAI.Application.Mappings;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Entities;
using MeandAI.Domain.Enums;
using MeandAI.Domain.Repositories;

namespace MeandAI.Application.Services;

public class UserLearningPathsService : IUserLearningPathsService
{
    private readonly IUserRepository _userRepository;
    private readonly ILearningPathRepository _learningPathRepository;
    private readonly IUserLearningPathRepository _userLearningPathRepository;

    public UserLearningPathsService(
        IUserRepository userRepository,
        ILearningPathRepository learningPathRepository,
        IUserLearningPathRepository userLearningPathRepository)
    {
        _userRepository = userRepository;
        _learningPathRepository = learningPathRepository;
        _userLearningPathRepository = userLearningPathRepository;
    }

    public async Task<UserLearningPathDto> EnrollAsync(Guid userId, EnrollUserInPathRequest request, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                   ?? throw new InvalidOperationException("Usuário não encontrado.");

        LearningPath learningPath = await _learningPathRepository.GetByIdAsync(request.LearningPathId, cancellationToken)
                          ?? throw new InvalidOperationException("Trilha de aprendizado não encontrada.");

        UserLearningPath? existing = await _userLearningPathRepository.GetByUserAndPathAsync(userId, request.LearningPathId, cancellationToken);
        if (existing is not null)
        {
            return existing.ToDto(learningPath.Name);
        }

        UserLearningPath enrollment = new UserLearningPath(userId, request.LearningPathId);
        await _userLearningPathRepository.AddAsync(enrollment, cancellationToken);

        return enrollment.ToDto(learningPath.Name);
    }

    public async Task<IReadOnlyCollection<UserLearningPathDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<UserLearningPath> enrollments = await _userLearningPathRepository.GetByUserAsync(userId, cancellationToken);
        if (enrollments.Count == 0)
        {
            return Array.Empty<UserLearningPathDto>();
        }

        Guid[] learningPathIds = enrollments.Select(e => e.LearningPathId).Distinct().ToArray();
        IReadOnlyCollection<LearningPath> learningPaths = await _learningPathRepository.GetByIdsAsync(learningPathIds, cancellationToken);
        Dictionary<Guid, string> learningPathDictionary = learningPaths.ToDictionary(lp => lp.Id, lp => lp.Name);

        List<UserLearningPathDto> dtos = enrollments
            .Select(e =>
            {
                string name = learningPathDictionary.GetValueOrDefault(e.LearningPathId, "Trilha não encontrada");
                return e.ToDto(name);
            })
            .ToList();

        return dtos;
    }

    public async Task<UserLearningPathDto?> UpdateStatusAsync(Guid userId, UpdateUserLearningPathStatusRequest request, CancellationToken cancellationToken = default)
    {
        UserLearningPath? enrollment = await _userLearningPathRepository.GetByIdAsync(request.UserLearningPathId, cancellationToken);
        if (enrollment is null || enrollment.UserId != userId)
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(LearningPathStatus), request.Status))
        {
            throw new InvalidOperationException("Status inválido para a trilha.");
        }

        enrollment.UpdateStatus(request.Status);
        await _userLearningPathRepository.UpdateAsync(enrollment, cancellationToken);

        LearningPath? learningPath = await _learningPathRepository.GetByIdAsync(enrollment.LearningPathId, cancellationToken);
        string name = learningPath?.Name ?? "Trilha não encontrada";
        return enrollment.ToDto(name);
    }
}
