using AzureSaga.Domain;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Repository
{
    public class SagaRepository : ISagaRepository
    {
        private IMongoCollection<Saga> _sagaCollection;
        private readonly ILogger<SagaRepository> _logger;
        public SagaRepository(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<SagaRepository> logger)
        {
            _logger = logger;
            var db = mongoClient.GetDatabase(mongoUrl?.DatabaseName);
            var collectionName = "Saga";
            _sagaCollection = db.GetCollection<Saga>(collectionName);
        }
        public async Task<Saga> InsertSagaAsync(Saga saga)
        {
            await _sagaCollection.InsertOneAsync(saga);
            return saga;
        }
        public async Task<Saga> GetSagaByIdAsync(string sagaId)
        {
            return (await _sagaCollection.FindAsync(saga => saga.Id == sagaId)).FirstOrDefault();
        }
        public async Task<bool> UpdateVotingRecordStatus(string sagaId, SagaState sagaState)
        {
            var saga = new Saga();
            var filter = Builders<Saga>.Filter.Eq(nameof(saga.Id), sagaId);
            var update = Builders<Saga>.Update.Set(nameof(saga.State), sagaState);
            var updateStatus = await _sagaCollection.UpdateOneAsync(filter, update);
            return updateStatus.ModifiedCount > 0;
        }
    }
}
