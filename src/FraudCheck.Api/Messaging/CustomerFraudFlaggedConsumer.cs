using FraudCheck.Contracts.Messaging;
using MassTransit;

namespace FraudCheck.Api.Messaging;

public class CustomerFraudFlaggedConsumer : IConsumer<CustomerFraudFlaggedMessage>
{
    private readonly ILogger<CustomerFraudFlaggedConsumer> _logger;

    public CustomerFraudFlaggedConsumer(ILogger<CustomerFraudFlaggedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CustomerFraudFlaggedMessage> context)
    {
        _logger.LogInformation(
            "Message {Message} received for action {EventType}",
            context.Message,
            context.Message.GetType().Name);

        // TODO: send email

        await Task.Delay(Random.Shared.Next(500));
    }
}
