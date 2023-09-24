using Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TurtlePublic.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class EventController : ControllerBase
{
    private readonly Event.IService events;
    public EventController(Event.IService events)
    {
        this.events = events;
    }

    /// <summary>
    /// Sums items gathered and lost within a certain window, with leaderboard
    /// stats.
    /// </summary>
    /// <param name="RangeStart">Start of the reporting window (down to the moment).</param>
    /// <param name="RangeEnd">End of the reporting window (down to the moment).</param>
    [HttpPost("Dashboard")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(Event.Dashboard))]
    public async Task<IActionResult> Dashboard(
        DateTime? RangeStart,
        DateTime? RangeEnd)
    {
        try
        {
            var result = await events.Dashboard(RangeStart, RangeEnd);
            return Ok(result);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Fetches the most recent hundred turtle events and their net material
    /// costs.
    /// </summary>
    [HttpPost("History")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Event.NetMaterial>))]
    public async Task<IActionResult> History()
    {
        try
        {
            var result = await events.History();
            return Ok(result);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Finds net material gains or losses within a certain window, grouped by
    /// material name.
    /// </summary>
    /// <param name="RangeStart">Start of the reporting window (down to the moment).</param>
    /// <param name="RangeEnd">End of the reporting window (down to the moment).</param>
    /// <returns></returns>
    [HttpPost("MaterialBreakdown")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Event.ByMaterial>))]
    public async Task<IActionResult> MaterialBreakdown(
        DateTime? RangeStart,
        DateTime? RangeEnd)
    {
        try
        {
            var result = await events.MaterialBreakdown(RangeStart, RangeEnd);
            return Ok(result);
        } catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}