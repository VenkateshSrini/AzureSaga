using AzureSaga.Domain;

namespace AzureSaga.Repository
{
    public interface ISagaRepository
    {
        Task<Saga> GetSagaByIdAsync(string sagaId);
        Task<Saga> InsertSagaAsync(Saga saga);
        Task<bool> UpdateVotingRecordStatus(string sagaId, SagaState sagaState);
    }
}