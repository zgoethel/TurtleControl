using Generated;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace TurtlePublic.Services;

public class TurtleService : Turtle.IBackendService
{
    private readonly Turtle.Repository repository;
    private readonly Account.IService accounts;
    private readonly ISshClientService sshClient;
    private readonly ILinkPathGenerator linkGenerator;
    private readonly IServiceProvider sp;

    private Package.IService _packages;
    private Package.IService packages =>
        (_packages ??= sp.GetRequiredService<Package.IService>());

    public TurtleService(Turtle.Repository repository, Account.IService accounts, ISshClientService sshClient, ILinkPathGenerator linkGenerator, IServiceProvider sp)
    {
        this.repository = repository;
        this.accounts = accounts;
        this.sshClient = sshClient;
        this.linkGenerator = linkGenerator;
        this.sp = sp;
    }

    private const string InvalidCheckTag = "Are you sure you're a turtle?";
    private const string GrandTheftTurtle = "These are not the droids you're looking for";
    private const string DeviceNotFound = "Failed to find that device";

    private static IDictionary<string, int> turtleOwners = new ConcurrentDictionary<string, int>();

    public async Task<Turtle.WithDetails> Get(int id)
    {
        return await repository.Turtle_GetById(id);
    }

    public async Task<Turtle.WithDetails> Get(int id, int _userId)
    {
        var turtle = await repository.Turtle_GetById(id);
        if (turtle is not null
            && !turtle.IsPublic && turtle.OwnerId != _userId)
        {
            throw new ApplicationException(GrandTheftTurtle);
        }
        return turtle;
    }

    public async Task<List<Turtle.WithOwner>> List(int page, int count, bool allUsers, int _userId)
    {
        return await repository.Turtle_List(page, count, allUsers ? 0 : _userId);
    }

    public Task<Turtle.CheckTagLink> BeginPairing(int _userId)
    {
        var randomness = Guid.NewGuid().ToString();
        var checkTagSuffix = randomness.Split("-")[0];
        var checkTag = linkGenerator.GenerateActionPath("p", checkTagSuffix);

        turtleOwners[checkTag] = _userId;

        return Task.FromResult(new Turtle.CheckTagLink()
        {
            Link = checkTag
        });
    }

    public async Task<Turtle> CheckPairing(string checkTag, int _userId)
    {
        if (!turtleOwners.TryGetValue(checkTag, out var owner))
        {
            throw new ApplicationException(InvalidCheckTag);
        }
        if (owner != _userId)
        {
            throw new ApplicationException(GrandTheftTurtle);
        }

        var lastSlash = checkTag.LastIndexOf("/");
        if (lastSlash == -1)
        {
            throw new ApplicationException(InvalidCheckTag);
        }
        var searchFor = checkTag.Substring(lastSlash + 1);

        var results = sshClient.FindFile(linkGenerator.CcRoot, searchFor);
        switch (results.Length)
        {
            case 0:
                throw new ApplicationException("Waiting for file to appear");
            case 1:
                turtleOwners.Remove(checkTag);
                break;
            default:
                turtleOwners.Remove(checkTag);
                throw new ApplicationException("Error: Duplicate files detected; manual cleanup required");
        }
        var path = results.Single()
            .Substring(linkGenerator.CcRoot.Length)
            .Trim('/');

        var pieces = path.Split('/');
        if (pieces.Length < 3)
        {
            throw new Exception("Error: Discovered CC path is too short to be correct");
        }
        if (!int.TryParse(pieces[1], out var ccNum))
        {
            throw new Exception("Error: Did not find CC identifier int in path");
        }
        path = $"{pieces[0]}/{pieces[1]}";

        var newTurtle = await repository.Turtle_Register("Unknown", ccNum, path, _userId);
        return newTurtle;
    }

    public async Task<string> GeneratePair(string checkTagSuffix)
    {
        var checkTag = linkGenerator.GenerateActionPath("p", checkTagSuffix);
        if (turtleOwners.TryGetValue(checkTag, out var owner))
        {
            var account = await accounts.Get(owner);
            return $"You wouldn't download a turtle, would you {account.FirstName}?\nThis ComputerCraft device should be discovered soon.";
        } else
        {
            throw new ApplicationException(InvalidCheckTag);
        }
    }

    public async Task<Turtle> Set(int id, int? cohortId, int _userId)
    {
        var existing = await Get(id, _userId);
        if (existing is null)
        {
            throw new ApplicationException(DeviceNotFound);
        }

        var turtle = await repository.Turtle_Set(id, cohortId);
        return turtle;
    }

    public async Task<Turtle> Share(int id, int _userId)
    {
        var existing = await repository.Turtle_GetById(id);
        if (existing is null || existing.OwnerId != _userId)
        {
            throw new ApplicationException(DeviceNotFound);
        }

        var turtle = await repository.Turtle_SetIsPublic(id, true);
        return turtle;
    }

    public async Task<Turtle> Unshare(int id, int _userId)
    {
        var existing = await repository.Turtle_GetById(id);
        if (existing is null || existing.OwnerId != _userId)
        {
            throw new ApplicationException(GrandTheftTurtle);
        }

        var turtle = await repository.Turtle_SetIsPublic(id, false);
        return turtle;
    }

    /// <summary>
    /// This is not secure, but is behind authenticated endpoints.
    /// </summary>
    public async Task<List<Turtle.SshFile>> ListFiles(int id, string path, int _userId)
    {
        var turtle = await Get(id, _userId);
        if (turtle is null)
        {
            throw new ApplicationException(DeviceNotFound);
        }

        var fullPath = linkGenerator.GenerateCcPath("computer",
            turtle.CCNum.ToString(),
            path);
        var listing = sshClient.ListFiles(fullPath);
        return listing;
    }

    /// <summary>
    /// This is not secure, but is behind authenticated endpoints.
    /// </summary>
    public async Task<Turtle.FileDownload> DownloadFile(int id, string path, string file, int _userId)
    {
        var turtle = await Get(id, _userId);
        if (turtle is null)
        {
            throw new ApplicationException(DeviceNotFound);
        }

        var fullPath = linkGenerator.GenerateCcPath("computer",
            turtle.CCNum.ToString(),
            path,
            file);
        var base64 = sshClient.DownloadFileBase64(fullPath);
        return new()
        {
            Base64Bytes = base64
        };
    }

    public async Task InstallPackage(int id, int packageId, int _userId)
    {
        var turtle = await Get(id, _userId);
        if (turtle is null)
        {
            throw new ApplicationException(DeviceNotFound);
        }

        // Security check
        _ = await packages.Get(packageId, _userId);

        await repository.Turtle_InstallPackage(id, packageId);
    }
}
