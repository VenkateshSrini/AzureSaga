using AzureSaga.Domain;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Repository
{
    public class UserCreditRepository : IUserCreditRepository
    {
        private IMongoCollection<UserCredit> _userCreditCollection;
        private readonly ILogger<UserCreditRepository> _logger;
        public UserCreditRepository(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<UserCreditRepository> logger)
        {
            _logger = logger;
            var db = mongoClient.GetDatabase(mongoUrl?.DatabaseName);
            var collectionName = "UserCredit";
            _userCreditCollection = db.GetCollection<UserCredit>(collectionName);
        }
        public async Task<List<UserCredit>> BulkInsertGames(List<UserCredit> userCredits)
        {
            var insertManyOptions = new InsertManyOptions();
            insertManyOptions.IsOrdered = true;
            try
            {
                await _userCreditCollection.InsertManyAsync(userCredits);
                return userCredits;
            }
            catch
            {
                throw;
            }

        }
        public async Task<UserCredit> GetUserCreditAsync(string userId)
        {
            var credit = await _userCreditCollection?.Find(credit => credit.UserId.Equals(userId))
                                                     .FirstOrDefaultAsync();
            return credit;

        }
        public async Task<bool> UpdateCreditAmountAsync(string userId, int expensedCredit)
        {
            var filter = Builders<UserCredit>.Filter.Eq("UserId", userId);
            var update = Builders<UserCredit>.Update.Inc("Credits", (expensedCredit * -1));
            var recordCount = await _userCreditCollection.UpdateOneAsync(filter, update);
            
            return recordCount.ModifiedCount > 0;
        }
    }
}
