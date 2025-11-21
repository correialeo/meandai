using MeandAI.Application.DTOs.Skills;

namespace MeandAI.Application.Services.Interfaces;

public interface ISkillsService
{
    Task<SkillDto> CreateAsync(CreateSkillRequest request, CancellationToken cancellationToken = default);
    Task<SkillDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SkillDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<SkillDto?> UpdateAsync(Guid id, UpdateSkillRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
