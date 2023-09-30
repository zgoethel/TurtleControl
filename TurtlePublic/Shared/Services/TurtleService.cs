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

    public async Task<Turtle.WithDetails> Get(int id)
    {
        return await repository.Turtle_GetById(id);
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
            throw new ApplicationException("Invalid check tag format");
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
                throw new ApplicationException("Duplicate files detected; manual cleanup required");
        }
        var path = results.Single()
            .Substring(linkGenerator.CcRoot.Length)
            .Trim('/');

        var pieces = path.Split('/');
        if (pieces.Length < 3)
        {
            throw new Exception("Discovered CC path is too short to be correct");
        }
        //TODO Set to unknown, capture in "firmware"
        var ccType = pieces[0].ToLower() switch
        {
            "computer" => "Computer",
            "turtle" => "Turtle",
            _ => throw new Exception($"Device folder type '{pieces[0]}' not currently supported")
        };
        if (!int.TryParse(pieces[1], out var ccNum))
        {
            throw new Exception("Did not find CC identifier int in path");
        }
        path = $"{pieces[0]}/{pieces[1]}";

        var newTurtle = await repository.Turtle_Register(ccType, ccNum, path, _userId);
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
}
