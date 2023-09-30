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
}