using Amazon.Runtime.Internal.Util;
using AzureSaga.Domain;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Repository
{
    public class UserRepository : IUserRepository
    {
        private IMongoCollection<User> _usersCollection;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(IMongoClient mongoClient, MongoUrl mongoUrl,
            ILogger<UserRepository> logger)
        {
            _logger = logger;
            var db = mongoClient.GetDatabase(mongoUrl?.DatabaseName);
            var collectionName = "Users";
            _usersCollection = db.GetCollection<User>(collectionName);

        }
        public async Task<List<User>> BulkLoadUsers(List<User> users)
        {
            var insertManyOptions = new InsertManyOptions();
            insertManyOptions.IsOrdered = true;
            try
            {
                await _usersCollection.InsertManyAsync(users);
                return users;
            }
            catch
            {
                throw;
            }
        }
        public async Task<User>GetUserById(string userID)
        {
            return await (await _usersCollection.FindAsync(user => user.Id.Equals(userID)))
                         ?.FirstOrDefaultAsync();
        }
    }
}
