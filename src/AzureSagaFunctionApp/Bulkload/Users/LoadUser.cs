using System.Net;
using Newtonsoft.Json;
using AzureSaga.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AzureSaga.Domain;

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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Starting the bulk load operation for Users.");
            var userJson = new StreamReader(req.Body).ReadToEnd();
            var users = JsonConvert.DeserializeObject<List<User>>(userJson);
            _userRepository.BulkLoadUsers(users);

            var userResponse = new { OperationStatus = 201, status = "User Loaded successfully" };
            var userResponseJson = JsonConvert.SerializeObject(userResponse);
            var bodyStream = new MemoryStream();
            var writer = new StreamWriter(bodyStream);
            writer.Write(userResponseJson);
            writer.Flush();
            bodyStream.Position = 0;
            var response = req.CreateResponse(HttpStatusCode.Created);

            response.Body = bodyStream;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            response.WriteString("completed the bulk load operation for Users.");

            return response;
        }
    }
}
