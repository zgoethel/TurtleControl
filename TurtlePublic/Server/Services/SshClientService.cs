using Renci.SshNet;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace TurtlePublic.Services;

public class SshClientService
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

    public string[] ListFiles(string path, string flags = "")
    {
        using var client = CreateClient();
        using var cmd = client.CreateCommand($"ls {flags} '{path}'");
        var output = cmd.Execute();
        return output.Split("\n", StringSplitOptions.RemoveEmptyEntries);
    }
}
