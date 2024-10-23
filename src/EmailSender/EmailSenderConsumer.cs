using Customer.Contracts.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EmailSender;

public class EmailSenderConsumer :
    IConsumer<CustomerCreatedMessage>,
    IConsumer<CustomerUpdatedMessage>,
    IConsumer<CustomerDeletedMessage>
{
    private readonly ILogger<EmailSenderConsumer> _logger;

    public EmailSenderConsumer(ILogger<EmailSenderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CustomerCreatedMessage> context)
    {
        _logger.LogInformation(
            "Message {Message} received for action {EventType}",
            context.Message,
            context.Message.GetType().Name);

        // TODO: send email

        await Task.Delay(Random.Shared.Next(500));
    }

    public async Task Consume(ConsumeContext<CustomerUpdatedMessage> context)
    {
        _logger.LogInformation(
            "Message {Message} received for action {EventType}",
            context.Message,
            context.Message.GetType().Name);

        // TODO: send email

        await Task.Delay(Random.Shared.Next(500));
    }

    public async Task Consume(ConsumeContext<CustomerDeletedMessage> context)
    {
        _logger.LogInformation(
            "Message {Message} received for action {EventType}",
            context.Message,
            context.Message.GetType().Name);

        // TODO: send email

        await Task.Delay(Random.Shared.Next(500));
    }
}
