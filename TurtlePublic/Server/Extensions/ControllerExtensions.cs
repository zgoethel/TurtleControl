using Microsoft.AspNetCore.Mvc;

namespace TurtlePublic.Extensions;

public static class ControllerExtensions
{
    public static int LoggedIn(this ControllerBase controller)
    {
        return int.Parse(controller.HttpContext.User.Claims.First((it) => it.Type == "Id").Value);
    }
}
