using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Skills;

public class GetSkillByIdUseCase
{
    private readonly ISkillsService _skillsService;

    public GetSkillByIdUseCase(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    public Task<SkillDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _skillsService.GetByIdAsync(id, cancellationToken);
    }
}
