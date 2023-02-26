using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace AzureSagaFunctionApp.Orchestration
{
    public static class AzureSagaOrchestrator
    {
        [Function(nameof(AzureSagaOrchestrator))]
        public static async Task<List<string>> RunOrchestrator(
            [Microsoft.Azure.Functions.Worker.OrchestrationTrigger] IDurableOrchestrationContext context, ILogger iLogger)
        {

            ILogger logger = context.CreateReplaySafeLogger(iLogger);



            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();
            //replace with entity triggers and not activity tiggers


            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }



        [Function("Function_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [Microsoft.Azure.Functions.Worker.DurableClient] IDurableClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("Function_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.StartNewAsync(
                nameof(AzureSagaOrchestrator));


            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
