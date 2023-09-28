namespace TurtlePublic.Services;

public interface ILinkPathGenerator
{
    string CcRoot { get; }
    string GenerateCcPath(params string[] pieces);
    string GenerateActionPath(params string[] pieces);
}
