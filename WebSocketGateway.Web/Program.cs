using Quartz;
using WebSocketGateway.Services.Abstractions;
using WebSocketGateway.Services.Abstractions.External;
using WebSocketGateway.Services.Implementations;
using WebSocketGateway.Services.Implementations.External;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IClientService, ClientService>();
builder.Services.AddSingleton<IActivityService, ActivityService>();
builder.Services.AddSingleton<IClientManagerBackgroundService, ClientManagerBackgroundService>();

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
