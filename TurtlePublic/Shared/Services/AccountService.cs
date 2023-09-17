using Generated;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

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
}
