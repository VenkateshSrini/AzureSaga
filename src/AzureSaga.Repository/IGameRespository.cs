using AzureSaga.Domain;

namespace AzureSaga.Repository
{
    public interface IGameRespository
    {
        Task<List<Game>> BulkInsertGames(List<Game> games);
    }
}