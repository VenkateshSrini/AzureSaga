using AzureSaga.Domain;

namespace AzureSaga.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> BulkLoadUsers(List<User> users);
    }
}