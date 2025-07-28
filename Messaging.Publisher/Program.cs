using MassTransit;
using Messaging.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(config =>
{
    config.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("AzureServiceBus:ConnectionString"));

        cfg.ConfigureEndpoints(context);

        cfg.Message<DayOfTheWeek>(e =>
        {
            e.SetEntityName("dayoftheweek-topic");
        });
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/publish", async (ILogger<Program> logger, IPublishEndpoint publishEndpoint) =>
{
    await publishEndpoint.Publish(new DayOfTheWeek { DayNumber = 1, Name = "Monday" },
        ctx =>
        {
            ctx.Headers.Set("Name", "Monday");
            ctx.Headers.Set("MessageType", nameof(DayOfTheWeek));
        });

    await publishEndpoint.Publish(new DayOfTheWeek { DayNumber = 1, Name = "Tuesday" },
        ctx =>
        {
            ctx.Headers.Set("Name", "Sunday");
        });

    logger.LogInformation("message sent");
})
.WithName("publish")
.WithOpenApi();

app.Run();
