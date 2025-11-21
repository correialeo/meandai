using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Skills;

public class CreateSkillUseCase
{
    private readonly ISkillsService _skillsService;

    public CreateSkillUseCase(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    public Task<SkillDto> ExecuteAsync(CreateSkillRequest request, CancellationToken cancellationToken = default)
    {
        return _skillsService.CreateAsync(request, cancellationToken);
    }
}
