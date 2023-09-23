using Generated;
using Microsoft.AspNetCore.Mvc;
using TurtlePublic.Extensions;
using TurtlePublic.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IModelDbAdapter, ModelDbAdapter>();
builder.Services.AddScoped<IModelDbWrapper, ModelDbWrapper>();
builder.Services.AddScoped<ILinkPathGenerator, LinkPathGenerator>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
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
    } else
    {
        config.AddServer(new()
        {
            Description = "Production",
            Url = "https://apps.jibini.net/TurtlePublic"
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
//app.MapFallbackToFile("index.html");
app.MapFallbackToPage("/_Host");

app.UseSwagger((config) =>
{
    config.RouteTemplate = "api/docs/{documentName}/swagger.json";
});
app.UseSwaggerUI((config) =>
{
    var hosting = app.Services.GetService<IWebHostEnvironment>();
    config.RoutePrefix = "api/docs";
    config.SwaggerEndpoint("v1/swagger.json", "TurtlePublic API v1");
    //config.SwaggerEndpoint("v1.1/swagger.json", "TurtlePublic API v1.1");
    config.EnableTryItOutByDefault();
});

app.Run();
