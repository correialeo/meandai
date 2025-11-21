using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Skills;

public class UpdateSkillUseCase
{
    private readonly ISkillsService _skillsService;

    public UpdateSkillUseCase(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    public Task<SkillDto?> ExecuteAsync(Guid skillId, UpdateSkillRequest request, CancellationToken cancellationToken = default)
    {
        return _skillsService.UpdateAsync(skillId, request, cancellationToken);
    }
}
