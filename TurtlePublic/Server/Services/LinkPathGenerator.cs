using System.Text;

namespace TurtlePublic.Services;

public class LinkPathGenerator : ILinkPathGenerator
{
    private IHttpContextAccessor httpContext;
    public LinkPathGenerator(IHttpContextAccessor httpContext)
    {
        this.httpContext = httpContext;
    }

    public string GenerateActionPath(params string[] pieces)
    {
        var path = new StringBuilder();
        var req = httpContext.HttpContext.Request;
        path.Append(req.Scheme);
        path.Append("://");
        path.Append(req.Host);
        if (!string.IsNullOrEmpty(req.PathBase.ToString().Trim('/')))
        {
            path.Append('/');
            path.Append(req.PathBase.ToString().Trim('/'));
        }

        foreach (var p in pieces)
        {
            path.Append('/');
            path.Append(p);
        }

        return path.ToString();
    }
}
