using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Skills;

public class GetSkillsByCategoryUseCase
{
    private readonly ISkillsService _skillsService;

    public GetSkillsByCategoryUseCase(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    public Task<IReadOnlyCollection<SkillDto>> ExecuteAsync(string category, CancellationToken cancellationToken = default)
    {
        return _skillsService.GetByCategoryAsync(category, cancellationToken);
    }
}
