using AzureSaga.Domain;
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
        public static async Task<StandardResponse> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(SagaOrchestrator));
            var userInput = context.GetInput<UserInputRequestMessage>();
            var standardResponse = new StandardResponse { SagaId = context.InstanceId };
            var userObj = await context.CallGetUserAsync(userInput.UserId);
            if (userObj is null)
            {
                standardResponse.OperationStatus = 400;
                standardResponse.Status = "User Id does not exist";
                return standardResponse;

            }
            if (userObj.Type!= UserType.Fan)
            {
                standardResponse.OperationStatus = 400;
                standardResponse.Status = "User should be a Fan to enter Saga";
                return standardResponse;
            }
            var gameObj = await context.CallGetGameAsync(userInput.GameId);
            if (gameObj is null)
            {
                standardResponse.OperationStatus = 400;
                standardResponse.Status = "Game id does not exist";
                return standardResponse;
            }
            var userCredit = await context.CallGetUserCreditInfoAsync(userInput.UserId);
            if (userCredit is null)
            {
                standardResponse.OperationStatus = 400;
                standardResponse.Status = "User credit does not exist";
                return standardResponse;
            }
            else if (userCredit.Credits < gameObj.MinCreditPoints)
            {
                standardResponse.OperationStatus = 400;
                standardResponse.Status = $"Game with id {userInput.GameId} requires minimum of {gameObj.MinCreditPoints} but user has only {userCredit.Credits}";
                return standardResponse;
            }
            else
            {
                var ucSagaIdUpdateStatus = await context.CallUserCreditSagaIdUpdateAsync(userInput.UserId);
                if (!ucSagaIdUpdateStatus)
                {
                    standardResponse.OperationStatus = 400;
                    standardResponse.Status = $"User credit update failed may be part of another saga";
                    return standardResponse;
                }
                var newVote = new Voting
                {
                    BettingPoint = userInput.VotingPoints,
                    GameId = userInput.GameId,
                    UserId = userInput.UserId,
                    SagaId = context.InstanceId,
                    RecordState = VotingRecordState.InProgress
                };
                var dbVote = await context.CallAddVoteAsync(newVote);
                if (dbVote is null)
                {
                    standardResponse.OperationStatus = 500;
                    standardResponse.Status = $"voting insert failed";
                    await context.CallResetUCSagaIDAsync(userInput.UserId);
                    return standardResponse;
                }
                var currentPoint = userCredit.Credits - userInput.VotingPoints;

                //(string userId, int points) updateCreditInput = 
                var updateUCPointsStatus = await context.CallUpdateUserCreditsAsync(
                   (userInput.UserId, currentPoint.Value));
                if (!updateUCPointsStatus)
                {
                    standardResponse.OperationStatus = 500;
                    standardResponse.Status = $"credit update failed";
                    await context.CallDeleteVotingAsync(dbVote.Id);
                    await context.CallResetUCSagaIDAsync(userInput.UserId);
                    return standardResponse;
                }
                var completeVotingStatus = await context.CallCompleteVotingAsync(userInput.UserId);
                if (!completeVotingStatus)
                {
                    standardResponse.OperationStatus = 500;
                    standardResponse.Status = $"voting status completion failed";
                    await context.CallDeleteVotingAsync(dbVote.Id);
                    await context.CallResetUCSagaIDAsync(userInput.UserId);

                }
                else
                {

                    await context.CallResetUCSagaIDAsync(userInput.UserId);
                    await context.CallResetVotingSagaAsync(dbVote.Id);
                    standardResponse.OperationStatus = 200;
                    standardResponse.Status = $"Saga Complete";
                }
            }

            return standardResponse;
        }



        [Function("HttpStart")]
        [OpenApiOperation(operationId: "HttpStart", tags: new[] { "StartOrchestration" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(UserInputRequestMessage))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SagaOrchestrator_HttpStart");
            var userInputJson = new StreamReader(req.Body).ReadToEnd();
            var userInputs = JsonConvert.DeserializeObject<UserInputRequestMessage>(userInputJson);
            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(SagaOrchestrator), userInputs);

            var standardResponse = new StandardResponse
            {
                OperationStatus = 200,
                Status = $"Started orchestration with ID = '{instanceId}'.",
                SagaId = instanceId
            };
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
