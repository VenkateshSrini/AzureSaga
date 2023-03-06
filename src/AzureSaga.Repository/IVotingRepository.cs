using AzureSaga.Domain;

namespace AzureSaga.Repository
{
    public interface IVotingRepository
    {
        Task<bool> DeleteVotingRecord(string votingId);
        Task<Voting> GetVotingByIdAsync(string votingId);
        Task<Voting> InsertVotingAsync(Voting voting);
        Task<bool> UpdateVotingRecordStatus(string votingId, VotingRecordState recordState);
        Task<bool> UpdateSagaId(string votingId, string sagaId);
    }
}