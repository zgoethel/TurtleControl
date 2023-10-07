using Generated;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TurtlePublic.Client;
using TurtlePublic.Extensions;
using TurtlePublic.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton((sp) => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress.TrimEnd('/') + "/api/v1/"),
    
});
builder.Services.AddScoped<IModelApiAdapter, ModelApiAdapter>();
builder.Services.AddScoped((sp) => sp.GetRequiredService<IModelApiAdapter>() as AuthenticationStateProvider);
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ToastService>();
builder.Services.AddTurtlePublicFrontend();

await builder.Build().RunAsync();
