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
    public class VotingRepository : IVotingRepository
    {
        private IMongoCollection<Voting> _votingCollection;
        private readonly ILogger<VotingRepository> _logger;
        public VotingRepository(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<VotingRepository> logger)
        {
            _logger = logger;
            var db = mongoClient.GetDatabase(mongoUrl?.DatabaseName);
            var collectionName = "Voting";
            _votingCollection = db.GetCollection<Voting>(collectionName);
        }
        public async Task<Voting> InsertVotingAsync(Voting voting)
        {
            await _votingCollection.InsertOneAsync(voting);
            return voting;
        }
        public async Task<Voting> GetVotingByIdAsync(string votingId)
        {
            return (await _votingCollection.FindAsync(vote => vote.Id == votingId)).FirstOrDefault();
        }
        public async Task<bool> UpdateVotingRecordStatus(string votingId, VotingRecordState recordState)
        {
            var filter = Builders<Voting>.Filter.Eq("Id", votingId);
            var update = Builders<Voting>.Update.Set("RecordState", recordState);
            var updateStatus = await _votingCollection.UpdateOneAsync(filter, update);
            return updateStatus.ModifiedCount > 0;
        }
        public async Task<bool> DeleteVotingRecord(string votingId)
        {
            var delResult = await _votingCollection.DeleteOneAsync(votingId);
            return delResult.DeletedCount > 0;
        }
    }
}
