using AuditSharp.PostgreSql.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuditSharp.PostgreSql.Extensions;

public static class Program
{
    public static IServiceCollection AddAuditSharp(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder)
    {
        services.AddDbContext<AuditSharpPostgreSqlDbContext>(optionsBuilder, ServiceLifetime.Transient, ServiceLifetime.Transient);
        return services;
    }

    public static IHost UseAuditSharp(this IHost host)
    {
        using var serviceScope = host.Services.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<AuditSharpPostgreSqlDbContext>();
        var pendingMigrations = context!.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }

        return host;
    }
}