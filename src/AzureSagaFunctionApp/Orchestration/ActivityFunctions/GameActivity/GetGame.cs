using AzureSaga.Domain;
using AzureSaga.Repository;
using AzureSagaFunctionApp.Orchestration.ActivityFunctions.UserCreditActivity;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.GameActivity
{
    [DurableTask(nameof(GetGame))]
    public class GetGame: TaskActivity<string, Game>
    {
        private readonly IGameRespository _gameRepository;
        private readonly ILogger<GetGame> _logger;
        public GetGame(IGameRespository gameRepository, ILogger<GetGame> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public override async Task<Game> RunAsync(TaskActivityContext context, string input)
        {
            return await _gameRepository.GetGameByID(input);
        }
    }
}
