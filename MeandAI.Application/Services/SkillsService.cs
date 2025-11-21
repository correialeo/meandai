using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Mappings;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;

namespace MeandAI.Application.Services;

public class SkillsService : ISkillsService
{
    private readonly ISkillRepository _skillRepository;

    public SkillsService(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<SkillDto> CreateAsync(CreateSkillRequest request, CancellationToken cancellationToken = default)
    {
        Skill skill = new Skill(request.Name, request.Category, request.Description);
        await _skillRepository.AddAsync(skill, cancellationToken);
        return skill.ToDto();
    }

    public async Task<SkillDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Skill? skill = await _skillRepository.GetByIdAsync(id, cancellationToken);
        return skill?.ToDto();
    }

    public async Task<IReadOnlyCollection<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Skill> skills = await _skillRepository.GetAllAsync(cancellationToken);
        return skills.Select(s => s.ToDto()).ToList();
    }

    public async Task<IReadOnlyCollection<SkillDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Skill> skills = await _skillRepository.GetByCategoryAsync(category, cancellationToken);
        return skills.Select(s => s.ToDto()).ToList();
    }

    public async Task<SkillDto?> UpdateAsync(Guid id, UpdateSkillRequest request, CancellationToken cancellationToken = default)
    {
        Skill? skill = await _skillRepository.GetByIdAsync(id, cancellationToken);
        if (skill is null)
        {
            return null;
        }

        skill.Update(request.Name, request.Category, request.Description);
        await _skillRepository.UpdateAsync(skill, cancellationToken);
        return skill.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Skill? skill = await _skillRepository.GetByIdAsync(id, cancellationToken);
        if (skill is null)
        {
            return false;
        }

        await _skillRepository.DeleteAsync(skill, cancellationToken);
        return true;
    }
}
