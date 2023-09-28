using Generated;
using System.Collections.Concurrent;

namespace TurtlePublic.Services;

public class TurtleService : Turtle.IBackendService
{
    private readonly Turtle.Repository repository;
    private readonly Account.IService accounts;
    private readonly ISshClientService sshClient;
    private readonly ILinkPathGenerator linkGenerator;
    public TurtleService(Turtle.Repository repository, Account.IService accounts, ISshClientService sshClient, ILinkPathGenerator linkGenerator)
    {
        this.repository = repository;
        this.accounts = accounts;
        this.sshClient = sshClient;
        this.linkGenerator = linkGenerator;
    }

    private const string InvalidCheckTag = "Are you sure you're a turtle?";
    private const string GrandTheftTurtle = "These are not the droids you're looking for";

    private static IDictionary<string, int> turtleOwners = new ConcurrentDictionary<string, int>();

    public Task<string> BeginPairing(int _userId)
    {
        var randomness = Guid.NewGuid().ToString();
        var checkTagSuffix = randomness.Split("-")[0];
        var checkTag = linkGenerator.GenerateActionPath("p", checkTagSuffix);

        turtleOwners[checkTag] = _userId;

        return Task.FromResult(checkTag);
    }

    public Task<Turtle> CheckPairing(string checkTag, int _userId)
    {
        if (!turtleOwners.Remove(checkTag, out var owner))
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
            throw new ApplicationException("Invalid check tag format");
        }
        var searchFor = checkTag.Substring(lastSlash + 1);

        var results = sshClient.FindFile(linkGenerator.CcRoot, searchFor);
        switch (results.Length)
        {
            case 0:
                throw new ApplicationException("Waiting for file to appear");
            case 1:
                break;
            default:
                throw new ApplicationException("Duplicate files detected; manual cleanup required");
        }

        return Task.FromResult(new Turtle()
        {
            RootPath = results.Single(),
            OwnerId = owner
        });
    }

    public async Task<string> GeneratePair(string checkTagSuffix)
    {
        var checkTag = linkGenerator.GenerateActionPath("p", checkTagSuffix);
        if (turtleOwners./*Remove*/TryGetValue(checkTag, out var owner))
        {
            var account = await accounts.Get(owner);
            return $"You wouldn't download a turtle, would you {account.FirstName}?\nThis device is paired with your account.";
        } else
        {
            throw new ApplicationException(InvalidCheckTag);
        }
    }
}
