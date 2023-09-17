using Generated;
using Microsoft.AspNetCore.Mvc;
using TurtlePublic.Extensions;
using TurtlePublic.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IModelDbAdapter, ModelDbAdapter>();
builder.Services.AddScoped<IModelDbWrapper, ModelDbWrapper>();
builder.Services.AddTurtlePublicBackend();

builder.Services.AddControllersWithViews((config) =>
{
    config.ValueProviderFactories.Insert(0, new JsonBodyValueProviderFactory());
});
builder.Services.Configure<ApiBehaviorOptions>((config) =>
{
    config.SuppressInferBindingSourcesForParameters = true;
    config.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddRazorPages();

builder.Services.AddApiVersioning((config) =>
{
    config.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer((config) =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen((config) =>
{
    string generateNestedName(Type type)
    {
        if (type.DeclaringType is null)
        {
            return type.Name;
        } else
        {
            var parentName = generateNestedName(type.DeclaringType);
            return $"{parentName}.{type.Name}";
        }
    }
    config.CustomSchemaIds(generateNestedName);

    if (builder.Environment.IsDevelopment())
    {
        config.AddServer(new()
        {
            Description = "Local",
            Url = "https://localhost:7201"
        });
        config.AddServer(new()
        {
            Description = "Staging",
            Url = "https://TurtlePublic-dev.jibini.net"
        });
    } else
    {
        config.AddServer(new()
        {
            Description = "Production",
            Url = "https://TurtlePublic.today"
        });
    }

    config.SwaggerDoc("v1",
        new()
        {
            Title = "TurtlePublic API",
            Version = "v1"
        });
    //config.SwaggerDoc("v1.1",
    //    new()
    //    {
    //        Title = "TurtlePublic API",
    //        Version = "v1.1"
    //    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
} else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseSwagger((config) =>
{
    config.RouteTemplate = "api/docs/{documentName}/swagger.json";
});
app.UseSwaggerUI((config) =>
{
    config.RoutePrefix = "api/docs";
    config.SwaggerEndpoint("/api/docs/v1/swagger.json", "TurtlePublic API v1");
    //config.SwaggerEndpoint("/api/docs/v1.1/swagger.json", "TurtlePublic API v1.1");
    config.EnableTryItOutByDefault();
});

app.Run();
