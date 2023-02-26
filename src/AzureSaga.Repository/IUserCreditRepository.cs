using AzureSaga.Domain;

namespace AzureSaga.Repository
{
    public interface IUserCreditRepository
    {
        Task<List<UserCredit>> BulkInsertUserCredits(List<UserCredit> userCredits);
        Task<UserCredit> GetUserCreditAsync(string userId);
        Task<bool> UpdateCreditAmountAsync(string userId, int expensedCredit);
    }
}