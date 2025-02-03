using Cagnaz.Family.Application.Repositorys;
using Cagnaz.Family.Infrastructure.Repositorys;
using Cagnaz.Family.Persistence.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cagnaz.Family.Infrastructure.Registration;

public static class Registration
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(10, 3, 35));
        services.AddDbContext<MySqlFamilyDataContext>(options =>
        {
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                serverVersion,
                options => options.EnableRetryOnFailure());
        });
      
    }
    public static void AddRepositorys(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}