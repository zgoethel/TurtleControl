using Org.BouncyCastle.Ocsp;
using System.Text;

namespace TurtlePublic.Services;

public class LinkPathGenerator : ILinkPathGenerator
{
    private IConfiguration config;
    private IHttpContextAccessor httpContext;
    public LinkPathGenerator(IConfiguration config, IHttpContextAccessor httpContext)
    {
        this.config = config;
        this.httpContext = httpContext;
    }

    public string CcRoot => config.GetValue<string>("Ssh:CcRoot");

    public string GenerateActionPath(params string[] pieces)
    {
        var path = new StringBuilder();
        var req = httpContext.HttpContext.Request;
        path.Append(/*req.Scheme*/"https");
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
            path.Append(p.Trim('/'));
        }

        return path.ToString();
    }

    public string GenerateCcPath(params string[] pieces)
    {
        var path = new StringBuilder();
        path.Append(CcRoot.TrimEnd('/'));

        foreach (var p in pieces)
        {
            path.Append('/');
            path.Append(p.Trim('/'));
        }

        return path.ToString();
    }
}
