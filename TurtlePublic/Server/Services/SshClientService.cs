using Generated;
using Renci.SshNet;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace TurtlePublic.Services;

/// <summary>
/// This isn't meant to be secure. It is not secure. Lots of unchecked
/// string concatenation, user-provided values, and bad path building.
/// 
/// <a href="https://mywiki.wooledge.org/ParsingLs">A reference.</a>
/// </summary>
public class SshClientService : ISshClientService
{
    private readonly IConfiguration config;
    public SshClientService(IConfiguration config)
    {
        this.config = config;
    }

    private ConnectionInfo CreateInfo()
    {
        var server = config.GetValue<string>("Ssh:Server");
        var port = config.GetValue<int>("Ssh:Port");
        var username = config.GetValue<string>("Ssh:Username");
        var password = config.GetValue<string>("Ssh:Password");

        var auth = new PasswordAuthenticationMethod(username, password);
        return new(server, port, username, auth);
    }

    private SshClient CreateClient()
    {
        var client = new SshClient(CreateInfo());
        client.HostKeyReceived += (sender, e) =>
        {
            var expectedSignature = config.GetValue<string>("Ssh:ExpectedSignature");
            var base64 = Convert.ToBase64String(e.FingerPrint);
            e.CanTrust &= expectedSignature.Equals(base64);
            if (!e.CanTrust)
            {
                throw new ApplicationException($"Did not trust received fingerprint '{base64}'");
            }
        };
        client.Connect();
        return client;
    }

    /// <summary>
    /// Insecure, but not accessible to anonymous users.
    /// </summary>
    public List<Turtle.SshFile> ListFiles(string path, string flags = "")
    {
        using var client = CreateClient();
        using var cmd = client.CreateCommand($"ls -F {flags} '{path}'");
        var output = cmd.Execute();
        return output.Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .Select((it) => new Turtle.SshFile()
            {
                IsDir = it.EndsWith("/"),
                Path = it.TrimEnd('*', '=', '|', '@')
            })
            .ToList();
    }

    /// <summary>
    /// Insecure, but never used with user-provided inputs.
    /// </summary>
    public string[] FindFile(string dir, string name)
    {
        using var client = CreateClient();
        using var cmd = client.CreateCommand($"find '{dir}' -type f -name '{name}'");
        var output = cmd.Execute();
        return output.Split("\n", StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Insecure, but not accessible to anonymous users.
    /// </summary>
    public string DownloadFileBase64(string path)
    {
        using var client = CreateClient();
        using var cmd = client.CreateCommand($"cat '{path}' | base64");
        var output = cmd.Execute();
        return output;
    }

    /// <summary>
    /// Insecure, but not accessible to anonymous users.
    /// </summary>
    public byte[] DownloadFile(string path)
    {
        var base64 = DownloadFileBase64(path);
        return Convert.FromBase64String(base64);
    }
}
