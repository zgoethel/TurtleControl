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
        path.Append(httpContext.HttpContext.Request.PathBase.ToString().TrimEnd('/'));
        
        foreach (var p in pieces)
        {
            path.Append('/');
            path.Append(p);
        }

        return path.ToString();
    }
}
