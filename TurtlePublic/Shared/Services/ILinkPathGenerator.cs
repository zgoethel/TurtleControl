namespace TurtlePublic.Services;

public interface ILinkPathGenerator
{
    string GenerateActionPath(params string[] pieces);
}
