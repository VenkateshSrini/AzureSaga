using AzureSaga.Repository;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker=>  worker.UseNewtonsoftJson())
    .ConfigureServices((hostBuilderContext, services)=>
    {
        services.AddMongoDbClient(hostBuilderContext.Configuration["ConnectionString"]);
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IGameRespository,GameRespository>();
        services.AddSingleton<IUserCreditRepository, UserCreditRepository>();
        services.AddSingleton<IVotingRepository, VotingRepository>();
        services.AddLogging();
        
    })
    .ConfigureOpenApi()
    .Build();

host.Run();