using System.Net;
using AzureSagaFunctionApp.MessagePackets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureSagaFunctionApp.Orchestration
{
    public class SagaEntrypoint
    {
        private readonly ILogger _logger;

        public SagaEntrypoint(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SagaEntrypoint>();
        }

        [Function("SagaEntrypoint")]
        [OpenApiOperation(operationId: "SagaEntrypoint", tags: new[] { "StartOrchestration" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(UserInputRequestMessage))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public OrchestrationStartResponse Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var userInputJson = new StreamReader(req.Body).ReadToEnd();
            var userInputs = JsonConvert.DeserializeObject<UserInputRequestMessage>(userInputJson);
            var response = new OrchestrationStartResponse
            {
                QueueInput = userInputs,
                Response = new StandardResponse
                {
                    OperationStatus = 200,
                    Status = $"queue updated with inputs'.",
                    SagaId = ""
                }
            };


            return response;
        }
    }
}
