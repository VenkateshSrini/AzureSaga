using AzureSaga.Domain;
using AzureSaga.Repository;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.VotingActivities
{
    [DurableTask(nameof(CompleteVoting))]
    public class CompleteVoting:TaskActivity<string,bool>
    {

        public readonly IVotingRepository _votingRepository;
        public readonly ILogger<AddVote> _logger;
        public CompleteVoting(IVotingRepository votingRepository, ILogger<AddVote> logger)
        {
            _votingRepository = votingRepository;
            _logger = logger;
        }

        public override async Task<bool> RunAsync(TaskActivityContext context, string input)
        {
           var voting = await _votingRepository.GetVotingByIdAsync(input);
            if(voting.SagaId.CompareTo(context.InstanceId)==0)
            {
                return await _votingRepository.UpdateVotingRecordStatus(input,VotingRecordState.Completed); 
            }
            return false;
        }
    }
}
