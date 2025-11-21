using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Skills;

public class GetAllSkillsUseCase
{
    private readonly ISkillsService _skillsService;

    public GetAllSkillsUseCase(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    public Task<IReadOnlyCollection<SkillDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _skillsService.GetAllAsync(cancellationToken);
    }
}
