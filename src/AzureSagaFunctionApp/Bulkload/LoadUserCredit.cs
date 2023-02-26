using System.Net;
using AzureSaga.Domain;
using AzureSaga.Repository;
using AzureSagaFunctionApp.MessagePackets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureSagaFunctionApp.Bulkload
{
    public class LoadUserCredit
    {
        private readonly ILogger _logger;
        private readonly IUserCreditRepository _userCreditRepository;

        public LoadUserCredit(ILoggerFactory loggerFactory, IUserCreditRepository userCreditRepository)
        {
            _logger = loggerFactory.CreateLogger<LoadUserCredit>();
            _userCreditRepository = userCreditRepository;
        }

        [Function("LoadUserCredit")]
        [OpenApiOperation(operationId: "BulkLoadUserCredit", tags: new[] { "LoadUserCredit" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(List<UserCredit>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public async Task<HttpResponseData> BulkLoadUserCredit([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Bulk Loading User Credits");

            var userCreditsJson = new StreamReader(req.Body).ReadToEnd();
            var userCredits = JsonConvert.DeserializeObject<List<UserCredit>>(userCreditsJson);
            await _userCreditRepository.BulkInsertUserCredits(userCredits);

            var userCreditResponse = new StandardResponse { OperationStatus = 201, Status = "Games Loaded successfully" };
            var userCreditResponseJson = JsonConvert.SerializeObject(userCreditResponse);
            var bodyStream = new MemoryStream();
            var writer = new StreamWriter(bodyStream);
            writer.Write(userCreditResponseJson);
            writer.Flush();
            bodyStream.Position = 0;
            var response = req.CreateResponse(HttpStatusCode.Created);

            response.Body = bodyStream;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");


            _logger.LogInformation("Completed of bulk loading of Games");


            return response;
        }
    }
}
