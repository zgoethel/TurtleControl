using Generated;

namespace TurtlePublic.Services;

public interface ISshClientService
{
    List<Turtle.SshFile> ListFiles(string path, string flags = "");

    string[] FindFile(string dir, string name);

    string DownloadFileBase64(string path);

    byte[] DownloadFile(string path);
}
