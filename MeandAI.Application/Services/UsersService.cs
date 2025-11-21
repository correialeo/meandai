using MeandAI.Application.Common;
using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Mappings;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Domain.Entities;
using MeandAI.Domain.Repositories;
using BCrypt.Net;

namespace MeandAI.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUserRepository _userRepository;
    private readonly ISkillRepository _skillRepository;

    public UsersService(IUserRepository userRepository, ISkillRepository skillRepository)
    {
        _userRepository = userRepository;
        _skillRepository = skillRepository;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        User? existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail {request.Email}.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        User user = new User(request.Name, request.Email, request.CurrentRole, request.DesiredArea, passwordHash);
        await _userRepository.AddAsync(user, cancellationToken);
        return user.ToDto();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user?.ToDto();
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user?.ToDto();
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<User> users = await _userRepository.GetPagedAsync(page, pageSize, cancellationToken);
        int total = await _userRepository.CountAsync(cancellationToken);
        List<UserDto> dtos = users.Select(u => u.ToDto()).ToList();
        return new PagedResult<UserDto>(dtos, page, pageSize, total);
    }

    public async Task<UserDto?> UpdateProfileAsync(Guid id, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.UpdateProfile(request.Name, request.CurrentRole, request.DesiredArea);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return user.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        await _userRepository.DeleteAsync(user, cancellationToken);
        return true;
    }

    public async Task<UserSkillDto?> AddSkillAsync(Guid userId, AddUserSkillRequest request, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        Skill? skill = await _skillRepository.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill is null)
        {
            throw new InvalidOperationException("A skill informada não foi encontrada.");
        }

        user.AddSkill(skill, request.Level);
        await _userRepository.UpdateAsync(user, cancellationToken);

        UserSkill userSkill = user.Skills.First(us => us.SkillId == request.SkillId);
        return userSkill.ToDto(skill);
    }

    public async Task<IReadOnlyCollection<UserSkillDto>> GetSkillsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.Skills.Any())
        {
            return Array.Empty<UserSkillDto>();
        }

        Guid[] skillIds = user.Skills.Select(us => us.SkillId).ToArray();
        IReadOnlyCollection<Skill> skills = await _skillRepository.GetByIdsAsync(skillIds, cancellationToken);
        Dictionary<Guid, Skill> skillDictionary = skills.ToDictionary(s => s.Id);

        List<UserSkillDto> dtos = user.Skills
            .Where(us => skillDictionary.ContainsKey(us.SkillId))
            .Select(us => us.ToDto(skillDictionary[us.SkillId]))
            .ToList();

        return dtos;
    }
}
