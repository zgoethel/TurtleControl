using Generated;
using Microsoft.Extensions.DependencyInjection;

namespace TurtlePublic.Services;

public class PackageService : Package.IBackendService
{
    private readonly Package.Repository repository;
    private readonly IServiceProvider sp;

    private Turtle.IService _turtles;
    private Turtle.IService turtles =>
        (_turtles ??= sp.GetRequiredService<Turtle.IService>());

    public PackageService(Package.Repository repository, IServiceProvider sp)
    {
        this.repository = repository;
        this.sp = sp;
    }

    private const string GrandTheftTurtle = "These are not the droids you're looking for";
    private const string PackageNotFound = "Failed to find that package";

    public async Task<Package> Get(int id)
    {
        return await repository.Package_GetById(id);
    }

    public async Task<Package> Get(int id, int _userId)
    {
        var package = await repository.Package_GetById(id);
        if (package is not null
            && !package.IsPublic && package.OwnerId != _userId)
        {
            throw new ApplicationException(GrandTheftTurtle);
        }
        return package;
    }

    public async Task<Package> Set(int id, string name, int _userId)
    {
        var existing = await Get(id, _userId);
        if (existing is null)
        {
            throw new ApplicationException(PackageNotFound);
        }

        var package = await repository.Package_Set(id, name, _userId);
        return package;
    }

    public async Task<Package> Share(int id, int _userId)
    {
        var existing = await repository.Package_GetById(id);
        if (existing is null || existing.OwnerId != _userId)
        {
            throw new ApplicationException(PackageNotFound);
        }

        var package = await repository.Package_SetIsPublic(id, true);
        return package;
    }

    public async Task<Package> Unshare(int id, int _userId)
    {
        var existing = await repository.Package_GetById(id);
        if (existing is null || existing.OwnerId != _userId)
        {
            throw new ApplicationException(PackageNotFound);
        }

        var package = await repository.Package_SetIsPublic(id, false);
        return package;
    }

    public async Task<Package.SourceVersion> AddSource(int id, string path, string source, int turtleId, int _userId)
    {
        var existing = await Get(id, _userId);
        if (existing is null)
        {
            throw new ApplicationException(PackageNotFound);
        }

        // Security check
        _ = await turtles.Get(turtleId, _userId);

        return await repository.Package_AddSource(id, path, source, turtleId, _userId);
    }

    public async Task<Package.SourceVersion> Commit(int id, string path, string source, string description, int _userId)
    {
        var existing = await Get(id, _userId);
        if (existing is null)
        {
            throw new ApplicationException(PackageNotFound);
        }

        return await repository.Package_Commit(id, path, description, _userId, source);
    }
}
