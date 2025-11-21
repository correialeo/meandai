using MeandAI.Application.Services;
using MeandAI.Application.Services.Interfaces;
using MeandAI.Application.UseCases.LearningPaths;
using MeandAI.Application.UseCases.Skills;
using MeandAI.Application.UseCases.UserLearningPaths;
using MeandAI.Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace MeandAI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISkillsService, SkillsService>();
        services.AddScoped<ILearningPathsService, LearningPathsService>();
        services.AddScoped<IUserLearningPathsService, UserLearningPathsService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtService, JwtService>();

        // Users use cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<GetUsersPagedUseCase>();
        services.AddScoped<UpdateUserProfileUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<AddSkillToUserUseCase>();
        services.AddScoped<GetUserSkillsUseCase>();

        // Skills use cases
        services.AddScoped<CreateSkillUseCase>();
        services.AddScoped<GetSkillByIdUseCase>();
        services.AddScoped<GetAllSkillsUseCase>();
        services.AddScoped<GetSkillsByCategoryUseCase>();
        services.AddScoped<UpdateSkillUseCase>();
        services.AddScoped<DeleteSkillUseCase>();

        // Learning paths use cases
        services.AddScoped<CreateLearningPathUseCase>();
        services.AddScoped<GetLearningPathByIdUseCase>();
        services.AddScoped<GetAllLearningPathsUseCase>();
        services.AddScoped<GetLearningPathsForTargetAreaUseCase>();
        services.AddScoped<UpdateLearningPathUseCase>();
        services.AddScoped<DeleteLearningPathUseCase>();
        services.AddScoped<AddStepToLearningPathUseCase>();

        // User learning paths use cases
        services.AddScoped<EnrollUserInLearningPathUseCase>();
        services.AddScoped<GetUserLearningPathsUseCase>();
        services.AddScoped<UpdateUserLearningPathStatusUseCase>();

        return services;
    }
}
