using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using Messaging.Consumer.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();

    config.AddConsumer<DayOfTheWeekConsumer>();

    config.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("AzureServiceBus:ConnectionString"), e => e.TransportType = Azure.Messaging.ServiceBus.ServiceBusTransportType.AmqpTcp);
        /*cfg.Message<DayOfTheWeek>(m =>
        {
            m.SetEntityName("configured-name");
        });*/

        cfg.SubscriptionEndpoint("day-of-week-subscription", "dayoftheweek-topic", e =>
        {
            e.ConfigureConsumer<DayOfTheWeekConsumer>(context, e => e.UseMessageRetry(x => x.Interval(5, TimeSpan.FromSeconds(5))));
            e.Filter = new SqlRuleFilter("Name = 'Monday'");
        });

        cfg.SubscriptionEndpoint("day-of-week-subscription2", "dayoftheweek-topic", e =>
        {
            e.ConfigureConsumer<DayOfTheWeekConsumer>(context, e => e.UseMessageRetry(x => x.Interval(5, TimeSpan.FromSeconds(2))));
            e.Filter = new SqlRuleFilter("Name <> 'Monday'");
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

app.Run();
