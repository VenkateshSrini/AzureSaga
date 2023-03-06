using AzureSagaFunctionApp.MessagePackets;
using AzureSagaFunctionApp.Orchestration.ActivityFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AzureSagaFunctionApp.Orchestration
{
    public static class SagaOrchestrator
    {
        [Function(nameof(SagaOrchestrator))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(SagaOrchestrator));
            var userInput = context.GetInput<UserInputRequestMessage>();
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();
            await context.CallActivityAsync<bool>(nameof(UserCreditSagaIdUpdate), userInput.UserId);
            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [Function(nameof(SayHello))]
        public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [Function("HttpStart")]
        [OpenApiOperation(operationId: "HttpStart", tags: new[] { "StartOrchestration" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(UserInputRequestMessage))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SagaOrchestrator_HttpStart");
            var userInputJson = new StreamReader(req.Body).ReadToEnd();
            var userInputs = JsonConvert.DeserializeObject<UserInputRequestMessage>(userInputJson);
            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(SagaOrchestrator), userInputs);

            var standardResponse = new StandardResponse { OperationStatus = 200, Status = $"Started orchestration with ID = '{instanceId}'." };
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(standardResponse), Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.Created,
                ReasonPhrase = "Orchestration started"
            };

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
