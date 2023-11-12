using Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurtlePublic.Extensions;

namespace TurtlePublic.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class PackageController : ControllerBase
{
    private readonly Package.IService packages;
    public PackageController(Package.IService packages)
    {
        this.packages = packages;
    }

    /// <summary>
    /// Retrieves details for a particular record.
    /// </summary>
    [HttpPost("Get")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Package))]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var _userId = this.LoggedIn();
            var package = await packages.Get(id, _userId);
            return Ok(package);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Updates details for a particular record, or else creating a new one.
    /// </summary>
    [HttpPost("Set")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Package))]
    public async Task<IActionResult> Set(int id, string name)
    {
        try
        {
            var _userId = this.LoggedIn();
            var package = await packages.Set(id, name, _userId);
            return Ok(package);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Sets a package to public; can only be performed by the owner.
    /// </summary>
    [HttpPost("Share")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Package))]
    public async Task<IActionResult> Share(int id)
    {
        try
        {
            var _userId = this.LoggedIn();
            var package = await packages.Share(id, _userId);
            return Ok(package);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Sets a package to private; can only be performed by the owner.
    /// </summary>
    [HttpPost("Unshare")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Package))]
    public async Task<IActionResult> Unshare(int id)
    {
        try
        {
            var _userId = this.LoggedIn();
            var package = await packages.Unshare(id, _userId);
            return Ok(package);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Links a Turtle file path to a package to be managed. Source must be
    /// further published to peer Turtles.
    /// </summary>
    [HttpPost("AddSource")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Package.SourceVersion))]
    public async Task<IActionResult> AddSource(int id, string path, string source, int turtleId)
    {
        try
        {
            var _userId = this.LoggedIn();
            var version = await packages.AddSource(id, path, source, turtleId, _userId);
            return Ok(version);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Updates the latest source for the managed file, marking related packages
    /// as dirty (requiring re-publishing).
    /// </summary>
    [HttpPost("Commit")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Package.SourceVersion))]
    public async Task<IActionResult> Commit(int id, string path, string source, string description)
    {
        try
        {
            var _userId = this.LoggedIn();
            var version = await packages.Commit(id, path, source, description, _userId);
            return Ok(version);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}