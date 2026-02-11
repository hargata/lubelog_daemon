using LubeLogDaemon.External;
using LubeLogDaemon.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ICachedReminders, CachedReminders>();
builder.Services.AddSingleton<IWebHookLogic, WebHookLogic>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
