using MeandAI.Domain.Repositories;
using MeandAI.Infrastructure.Data;
using MeandAI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MeandAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MeandAIDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ILearningPathRepository, LearningPathRepository>();
        services.AddScoped<IUserLearningPathRepository, UserLearningPathRepository>();

        return services;
    }
}
