using Quartz;
using WebSocketGateway.Services.Abstractions;
using WebSocketGateway.Services.Abstractions.External;
using WebSocketGateway.Services.Configuration;
using WebSocketGateway.Services.Implementations;
using WebSocketGateway.Services.Implementations.External;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration as IConfiguration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Configure External Services

builder.Services.AddSingleton<IActivityServiceHttpConfiguration>((x) =>
    configuration
        .GetSection("ActivityServiceHttpConfiguration")
        .Get<ActivityServiceHttpConfiguration>());
builder.Services.AddSingleton<IClientServiceHttpConfiguration>((x) =>
    configuration
        .GetSection("ClientServiceHttpConfiguration")
        .Get<ClientServiceHttpConfiguration>());

builder.Services.AddScoped<IClientService, ClientServiceHttp>();
builder.Services.AddScoped<IActivityService, ActivityServiceHttp>();
builder.Services.AddSingleton<IClientManagerBackgroundService, ClientManagerBackgroundService>();

// Add RabbitMq Service

builder.Services.AddHostedService<RabbitMqService>();

builder.Services.AddQuartz((q) =>
{
    var jobKey = new JobKey("ConnectionCheckJob");

    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<ScheduledConnectionCheck>(opts =>
    {
        opts.WithIdentity(jobKey);
    });

    q.AddTrigger(opts =>
    {
        opts
            .ForJob(jobKey)
            .WithIdentity("ConnectionCheck-Trigger")
            .WithCronSchedule("0/15 * * * * ?");
    });
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
