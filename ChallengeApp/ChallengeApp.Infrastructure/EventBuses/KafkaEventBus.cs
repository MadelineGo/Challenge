using System.Text.Json;
using ChallengeApp.Domain.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace ChallengeApp.Infrastructure.EventBuses;

public class KafkaEventBus : IEventBus
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus> _logger;

    public KafkaEventBus(IConfiguration configuration,
        ILogger<KafkaEventBus> logger)
    {
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        }).Build();

        _logger = logger;
    }

    public void Dispose() => _producer?.Dispose();

    public async Task Publish<T>(T @event) where T : EventMessage
    {
        var topicName = "permissions";
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event)
        };

        try
        {
            var response = await _producer.ProduceAsync(topicName, message);

            _logger.LogInformation("Message '{Key}' sent to topic '{TopicPartitionOffset}'", message.Key, response.TopicPartitionOffset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError("An error occurred while publishing message to Kafka: {Reason}", ex.Error.Reason);
        }
    }
}