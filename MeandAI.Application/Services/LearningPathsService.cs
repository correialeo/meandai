using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Mappings;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;

namespace MeandAI.Application.Services;

public class LearningPathsService : ILearningPathsService
{
    private readonly ILearningPathRepository _learningPathRepository;

    public LearningPathsService(ILearningPathRepository learningPathRepository)
    {
        _learningPathRepository = learningPathRepository;
    }

    public async Task<LearningPathDto> CreateAsync(CreateLearningPathRequest request, CancellationToken cancellationToken = default)
    {
        LearningPath learningPath = new LearningPath(request.Name, request.TargetArea, request.Description);

        if (request.Steps is not null)
        {
            foreach (CreateLearningPathStepRequest step in request.Steps)
            {
                learningPath.AddStep(step.Title, step.Description, step.ExternalCourseUrl, step.Order, step.EstimatedHours);
            }
        }

        await _learningPathRepository.AddAsync(learningPath, cancellationToken);
        return learningPath.ToDto();
    }

    public async Task<LearningPathDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        LearningPath? learningPath = await _learningPathRepository.GetByIdAsync(id, cancellationToken);
        return learningPath?.ToDto();
    }

    public async Task<IReadOnlyCollection<LearningPathDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<LearningPath> learningPaths = await _learningPathRepository.GetAllAsync(cancellationToken);
        return learningPaths.Select(lp => lp.ToDto()).ToList();
    }

    public async Task<IReadOnlyCollection<LearningPathDto>> GetForTargetAreaAsync(string targetArea, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<LearningPath> learningPaths = await _learningPathRepository.GetForTargetAreaAsync(targetArea, cancellationToken);
        return learningPaths.Select(lp => lp.ToDto()).ToList();
    }

    public async Task<LearningPathDto?> UpdateAsync(Guid id, UpdateLearningPathRequest request, CancellationToken cancellationToken = default)
    {
        LearningPath? learningPath = await _learningPathRepository.GetByIdAsync(id, cancellationToken);
        if (learningPath is null)
        {
            return null;
        }

        learningPath.Update(request.Name, request.TargetArea, request.Description);
        await _learningPathRepository.UpdateAsync(learningPath, cancellationToken);
        return learningPath.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        LearningPath? learningPath = await _learningPathRepository.GetByIdAsync(id, cancellationToken);
        if (learningPath is null)
        {
            return false;
        }

        await _learningPathRepository.DeleteAsync(learningPath, cancellationToken);
        return true;
    }

    public async Task<LearningPathStepDto?> AddStepAsync(Guid pathId, AddStepToPathRequest request, CancellationToken cancellationToken = default)
    {
        LearningPath? learningPath = await _learningPathRepository.GetByIdAsync(pathId, cancellationToken);
        if (learningPath is null)
        {
            return null;
        }

        LearningPathStep step = learningPath.AddStep(request.Title, request.Description, request.ExternalCourseUrl, request.Order, request.EstimatedHours);
        await _learningPathRepository.UpdateAsync(learningPath, cancellationToken);
        return step.ToDto();
    }
}
