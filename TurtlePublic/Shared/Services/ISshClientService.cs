namespace TurtlePublic.Services;

public interface ISshClientService
{
    string[] ListFiles(string path, string flags = "");

    string[] FindFile(string dir, string name);
}
