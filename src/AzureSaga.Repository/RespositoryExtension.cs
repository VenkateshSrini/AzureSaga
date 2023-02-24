using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AzureSaga.Repository
{
    public static class RespositoryExtension
    {
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, 
            string connectionString)
        { 
            var mongourl = MongoUrl.Create(connectionString);
           
            var mongoClient = new MongoClient(mongourl);
            services.AddSingleton<MongoUrl>(mongourl);
            services.AddSingleton<IMongoClient>(mongoClient);
            return services;
        }
    }
}