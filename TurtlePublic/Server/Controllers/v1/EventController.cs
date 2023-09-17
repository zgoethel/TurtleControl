using Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurtlePublic.Server.Services;

namespace TurtlePublic.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class EventController : ControllerBase
{
    private readonly Event.IService events;
    public EventController(Event.IService events)
    {
        this.events = events;
    }

    [HttpPost("Dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Event.Dashboard))]
    public async Task<IActionResult> Dashboard(
        DateTime? RangeStart,
        DateTime? RangeEnd)
    {
        var result = await events.Dashboard(RangeStart, RangeEnd);
        return Ok(result);
    }
}