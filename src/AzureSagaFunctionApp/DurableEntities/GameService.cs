using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureSaga.Domain;
using AzureSaga.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
namespace AzureSagaFunctionApp.DurableEntities
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GameService
    {
        [JsonProperty("CurrentGame")]
        internal Game? _currentGame;
        private readonly IGameRespository _gameRespository;
        public GameService(IGameRespository gameRespository)
        {
            _gameRespository = gameRespository;
        }
        public async Task<Game> GetGameAsync(string gameId)
        {
          if (_currentGame==default)
                _currentGame = await _gameRespository.GetGameByID(gameId);
          return _currentGame;
        }
        [FunctionName(nameof(GameService))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
       => ctx.DispatchAsync<GameService>();

    }
}
