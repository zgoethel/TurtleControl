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

    [HttpPost("ListFiles")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<string>))]
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
}