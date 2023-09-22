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

    public Task<Event.Dashboard> Dashboard(DateTime? RangeStart, DateTime? RangeEnd)
    {
        return repo.dbo__Event_Dashboard(RangeStart, RangeEnd);
    }

    public Task<List<Event.NetMaterial>> History()
    {
        return repo.dbo__Event_History();
    }

    public Task<List<Event.ByMaterial>> MaterialBreakdown(DateTime? RangeStart, DateTime? RangeEnd)
    {
        return repo.dbo__EventMaterial_ByMaterial(RangeStart, RangeEnd);
    }
}
