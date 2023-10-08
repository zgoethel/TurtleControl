using Microsoft.AspNetCore.Components;

namespace TurtlePublic.Services;

public class Toast
{
    public string Message { get; set; }
    public RenderFragment CustomContent { get; set; }
    public int Timeout { get; set; } = 6000;
    public bool CanCancel { get; set; } = true;
}

public class ToastService
{
    public delegate Task ToastCreatedAsync(Toast toast);
    public event ToastCreatedAsync OnToastCreated;

    public async Task CreateToastAsync(Toast toast)
    {
        if (OnToastCreated is not null)
        {
            await OnToastCreated.Invoke(toast);
        }
    }
}
