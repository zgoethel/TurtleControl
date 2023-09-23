using Generated;
using Microsoft.Extensions.Logging;

namespace TurtlePublic.Services;

public class EventService : Event.IBackendService
{
    private readonly ILogger<EventService> logger;
    private readonly Event.Repository repo;
    public EventService(ILogger<EventService> logger, Event.Repository repo)
    {
        this.logger = logger;
        this.repo = repo;
    }

    private void ValidateReasonableRange(DateTime? RangeStart, DateTime? RangeEnd)
    {
        if (RangeStart.HasValue && RangeEnd.HasValue && RangeEnd - RangeStart > TimeSpan.FromDays(7))
        {
            throw new Exception("Reporting windows can span at most 7 days");
        }
    }

    public Task<Event.Dashboard> Dashboard(DateTime? RangeStart, DateTime? RangeEnd)
    {
        ValidateReasonableRange(RangeStart, RangeEnd);
        return repo.dbo__Event_Dashboard(RangeStart, RangeEnd);
    }

    public Task<List<Event.NetMaterial>> History()
    {
        return repo.dbo__Event_History();
    }

    public Task<List<Event.ByMaterial>> MaterialBreakdown(DateTime? RangeStart, DateTime? RangeEnd)
    {
        ValidateReasonableRange(RangeStart, RangeEnd);
        return repo.dbo__EventMaterial_ByMaterial(RangeStart, RangeEnd);
    }
}
