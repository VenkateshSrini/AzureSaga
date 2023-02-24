using AzureSaga.Domain;

namespace AzureSaga.Repository
{
    public interface IUserCreditRepository
    {
        Task<List<UserCredit>> BulkInsertGames(List<UserCredit> userCredits);
    }
}