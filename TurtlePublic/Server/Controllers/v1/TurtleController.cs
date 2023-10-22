using Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurtlePublic.Extensions;

namespace TurtlePublic.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class TurtleController : ControllerBase
{
    private readonly Turtle.IService turtles;
    public TurtleController(Turtle.IService turtles)
    {
        this.turtles = turtles;
    }

    /// <summary>
    /// Retrieves a turtle, verifying access rights. Returns null if the device
    /// is not found.
    /// </summary>
    [HttpPost("Get")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Turtle.WithDetails>))]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var _userId = this.LoggedIn();
            var turtle = await turtles.Get(id, _userId);
            return Ok(turtle);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Lists all available devices in pages, filterable to items owned by the
    /// current user account.
    /// </summary>
    [HttpPost("List")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Turtle.WithOwner>))]
    public async Task<IActionResult> List(int page, int count, bool allUsers)
    {
        try
        {
            var _userId = this.LoggedIn();
            var list = await turtles.List(page, count, allUsers, _userId);
            return Ok(list);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Registers a new check tag which can be redeemed to pair a CC device.
    /// </summary>
    [HttpPost("BeginPairing")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(string))]
    public async Task<IActionResult> BeginPairing()
    {
        try
        {
            var _userId = this.LoggedIn();
            var returnPath = await turtles.BeginPairing(_userId);
            return Ok(returnPath);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Recursively searches the ComputerCraft save files for a file downloaded
    /// within the game. Pairs any discovered device.
    /// </summary>
    [HttpPost("CheckPairing")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Turtle))]
    public async Task<IActionResult> CheckPairing(string checkTag)
    {
        try
        {
            var _userId = this.LoggedIn();
            var turtle = await turtles.CheckPairing(checkTag, _userId);
            return Ok(turtle);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Updates turtle configuration values, including only details which should
    /// be accessible on public shares.
    /// </summary>
    [HttpPost("Set")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Turtle))]
    public async Task<IActionResult> Set(int id, int? cohortId)
    {
        try
        {
            var _userId = this.LoggedIn();
            var turtle = await turtles.Set(id, cohortId, _userId);
            return Ok(turtle);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Sets a turtle to public; can only be performed by the owner.
    /// </summary>
    [HttpPost("Share")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Turtle))]
    public async Task<IActionResult> Share(int id)
    {
        try
        {
            var _userId = this.LoggedIn();
            var turtle = await turtles.Share(id, _userId);
            return Ok(turtle);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Sets a turtle to private; can only be performed by the owner.
    /// </summary>
    [HttpPost("Unshare")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Turtle))]
    public async Task<IActionResult> Unshare(int id)
    {
        try
        {
            var _userId = this.LoggedIn();
            var turtle = await turtles.Unshare(id, _userId);
            return Ok(turtle);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a list of files in the ComputerCraft save data for the
    /// specific device at the specific sub-path. Insecure.
    /// </summary>
    [HttpPost("ListFiles")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Turtle.SshFile>))]
    public async Task<IActionResult> ListFiles(int id, string path)
    {
        try
        {
            var _userId = this.LoggedIn();
            var files = await turtles.ListFiles(id, path, _userId);
            return Ok(files);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves the contents of a specific file as Base64. Insecure.
    /// </summary>
    [HttpPost("DownloadFile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Turtle.FileDownload))]
    public async Task<IActionResult> DownloadFile(int id, string path, string file)
    {
        try
        {
            var _userId = this.LoggedIn();
            var files = await turtles.DownloadFile(id, path, file, _userId);
            return Ok(files);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Marks a particular package for installation on a turtle. Does not copy files.
    /// </summary>
    [HttpPost("InstallPackage")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> InstallPackage(int id, int packageId)
    {
        try
        {
            var _userId = this.LoggedIn();
            await turtles.InstallPackage(id, packageId, _userId);
            return Ok();
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}