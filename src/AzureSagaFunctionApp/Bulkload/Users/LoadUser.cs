using System.Net;
using Newtonsoft.Json;
using AzureSaga.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AzureSaga.Domain;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using AzureSagaFunctionApp.MessagePackets;

namespace AzureSagaFunctionApp.Bulkload.Users
{
    public class LoadUser
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public LoadUser(ILoggerFactory loggerFactory, IUserRepository userRepository)
        {
            _logger = loggerFactory.CreateLogger<LoadUser>();
            _userRepository = userRepository;
        }

        [Function("LoadUser")]
        [OpenApiOperation(operationId: "BulkLoadUsers", tags: new[] { "LoadUser" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json",typeof(List<User>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public async Task<HttpResponseData> BulkLoadUsers([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Starting the bulk load operation for Users.");
            var userJson = new StreamReader(req.Body).ReadToEnd();
            var users = JsonConvert.DeserializeObject<List<User>>(userJson);
            await _userRepository.BulkLoadUsers(users);

            var userResponse = new StandardResponse { OperationStatus = 201, Status = "User Loaded successfully" };
            var userResponseJson = JsonConvert.SerializeObject(userResponse);
            var bodyStream = new MemoryStream();
            var writer = new StreamWriter(bodyStream);
            writer.Write(userResponseJson);
            writer.Flush();
            bodyStream.Position = 0;
            var response = req.CreateResponse(HttpStatusCode.Created);

            response.Body = bodyStream;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            _logger.LogInformation("completed the bulk load operation for Users.");

            return response;
        }
    }
}
