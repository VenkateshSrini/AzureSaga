using System;
using AzureSagaFunctionApp.MessagePackets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureSagaFunctionApp.Orchestration
{
    public class SagaQueueStart
    {
        private readonly ILogger _logger;

        public SagaQueueStart(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SagaQueueStart>();
        }

        [Function("SagaQueueStart")]
        public async Task Run([QueueTrigger("saga-queue", Connection = "queue-con")] string userInputJson,
            [DurableClient] DurableTaskClient client)
        {
            var userInputs = JsonConvert.DeserializeObject<UserInputRequestMessage>(userInputJson);
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(SagaOrchestrator), userInputs);
            _logger.LogInformation($"C# Queue trigger function processed: {userInputJson}");
        }
    }
}
