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
    public class LoadGames
    {
        private readonly ILogger _logger;
        private IGameRespository _gameRespository;

        public LoadGames(ILoggerFactory loggerFactory, IGameRespository gameRespository)
        {
            _logger = loggerFactory.CreateLogger<LoadGames>();
            _gameRespository = gameRespository;
        }

        [Function("LoadGames")]
        [OpenApiOperation(operationId: "BulkLoadGames", tags: new[] { "LoadGames" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(List<Game>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StandardResponse), Description = "The OK response")]
        public async Task<HttpResponseData> BulkLoadGames([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Starting of bulk loading of Games");

            var gamesJson = new StreamReader(req.Body).ReadToEnd();
            var games = JsonConvert.DeserializeObject<List<Game>>(gamesJson);
            await _gameRespository.BulkInsertGames(games);

            var gameResponse = new StandardResponse { OperationStatus = 201, Status = "Games Loaded successfully" };
            var gameResponseJson = JsonConvert.SerializeObject(gameResponse);
            var bodyStream = new MemoryStream();
            var writer = new StreamWriter(bodyStream);
            writer.Write(gameResponseJson);
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
