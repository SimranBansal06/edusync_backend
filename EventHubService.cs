using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace webapi.Services
{
    public class EventHubService
    {
        private readonly string _connectionString;
        private readonly string _eventHubName;

        public EventHubService(IConfiguration configuration)
        {
            _connectionString = configuration["EventHub:ConnectionString"];
            _eventHubName = configuration["EventHub:Name"];
        }

        public async Task SendQuizEventAsync(object eventData)
        {
            await using var producerClient = new EventHubProducerClient(_connectionString, _eventHubName);
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
            string json = JsonSerializer.Serialize(eventData);
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json)));
            await producerClient.SendAsync(eventBatch);
        }
    }
}