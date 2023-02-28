using Amazon.Runtime.Internal.Util;
using AzureSaga.Domain;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Repository
{
    public class GameRespository : IGameRespository
    {
        private IMongoCollection<Game> _gamesCollection;
        private readonly ILogger<GameRespository> _logger;
        public GameRespository(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<GameRespository> logger)
        {
            _logger = logger;
            var db = mongoClient.GetDatabase(mongoUrl?.DatabaseName);
            var collectionName = "Games";
            _gamesCollection = db.GetCollection<Game>(collectionName);
        }
        public async Task<List<Game>> BulkInsertGames(List<Game> games)
        {
            var insertManyOptions = new InsertManyOptions();
            insertManyOptions.IsOrdered = true;
            try
            {
                await _gamesCollection.InsertManyAsync(games);
                return games;
            }
            catch
            {
                throw;
            }

        }
        public async Task<Game>GetGameByID(string GameId)
        {
            return await (await _gamesCollection.FindAsync(game=>game.Id.Equals(GameId)))
                         .FirstOrDefaultAsync();
        }

    }
}
