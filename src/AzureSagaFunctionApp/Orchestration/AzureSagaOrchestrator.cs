using AzureSagaFunctionApp.DurableEntities;
using AzureSagaFunctionApp.MessagePackets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AzureSagaFunctionApp.Orchestration
{
    public static class AzureSagaOrchestrator
    {
        [Function(nameof(AzureSagaOrchestrator))]
        public static async Task<bool> RunOrchestrator(
            [Microsoft.Azure.Functions.Worker.OrchestrationTrigger] IDurableOrchestrationContext context, ILogger iLogger)
        {

            ILogger logger = context.CreateReplaySafeLogger(iLogger);
            var userInput = context.GetInput<UserInputRequestMessage>();
            var gameEntityId = new EntityId(nameof(GameService), userInput.GameId);
            var userCredit = new EntityId(nameof(UserCreditService), $"{userInput.UserId}_{userInput.GameId}");
            var gameObj = context.CallEntityAsync(gameEntityId, "GetGameAsync", userInput.GameId);


            //replace with entity triggers and not activity tiggers


            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return true;
        }



        [Function("StartOrchestration")]
        [OpenApiOperation(operationId: "HttpStart", tags: new[] { "StartOrchestration" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(UserInputRequestMessage))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
             [Microsoft.Azure.Functions.Worker.DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("StartOrchestration");
            var userInputJson = new StreamReader(req.Body).ReadToEnd();
            var userInputs = JsonConvert.DeserializeObject<UserInputRequestMessage>(userInputJson);
            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(AzureSagaOrchestrator), userInputs)
                ;
           // await client.WaitForInstanceCompletionAsync(instanceId);
            var standardResponse = new StandardResponse { OperationStatus = 200, Status = $"Started orchestration with ID = '{instanceId}'." };
            var response = new HttpResponseMessage { 
                Content = new StringContent(JsonConvert.SerializeObject(standardResponse),Encoding.UTF8,"application/json"),
                StatusCode=HttpStatusCode.Created,ReasonPhrase="Orchestration started"};
            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            return response;
            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            //return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
