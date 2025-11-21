using MeandAI.Application.Services.Interfaces;

namespace MeandAI.Application.UseCases.Skills;

public class DeleteSkillUseCase
{
    private readonly ISkillsService _skillsService;

    public DeleteSkillUseCase(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    public Task<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _skillsService.DeleteAsync(id, cancellationToken);
    }
}
