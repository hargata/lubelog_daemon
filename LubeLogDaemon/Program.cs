using LubeLogDaemon.External;
using LubeLogDaemon.Logic;
using LubeLogDaemon.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("config/daemonConfig.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddSingleton<ICachedReminders, CachedReminders>();
builder.Services.AddSingleton<IWebHookLogic, WebHookLogic>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var backgroundServiceEnabled = bool.Parse(builder.Configuration[nameof(DaemonConfig.CheckDateReminders)] ?? "False");
if (backgroundServiceEnabled)
{
    builder.Services.AddHostedService<WebHookTimer>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
