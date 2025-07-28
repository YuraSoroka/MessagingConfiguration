using MassTransit;
using Messaging.Contracts;

namespace Messaging.Consumer.Consumers;

public class DayOfTheWeekConsumer(ILogger<DayOfTheWeekConsumer> logger) : IConsumer<DayOfTheWeek>
{
    public Task Consume(ConsumeContext<DayOfTheWeek> context)
    {
        logger.LogInformation($"Received DayOfTheWeek: {context.Message.DayNumber} - {context.Message.Name}");
        return Task.CompletedTask;
    }
}
