using Generated;
using Microsoft.Extensions.DependencyInjection;
using TurtlePublic.Services;

namespace TurtlePublic.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTurtlePublicBackend(this IServiceCollection services)
    {
        services.AddEventBackend<EventService>();
        services.AddAccountBackend<AccountService>();
        services.AddTurtleBackend<TurtleService>();
        services.AddPackageBackend<PackageService>();
    }

    public static void AddTurtlePublicFrontend(this IServiceCollection services)
    {
        services.AddEventFrontend();
        services.AddAccountFrontend();
        services.AddTurtleFrontend();
        services.AddPackageFrontend();
    }
}
