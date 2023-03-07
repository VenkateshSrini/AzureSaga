using AzureSaga.Repository;
using AzureSagaFunctionApp.Orchestration.ActivityFunctions.UserCreditActivity;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.VotingActivities
{
    [DurableTask(nameof(DeleteVoting))]
    public class DeleteVoting:TaskActivity<string,bool>
    {
        public readonly IVotingRepository _votingRepository;
        public readonly ILogger<AddVote> _logger;

        public override async Task<bool> RunAsync(TaskActivityContext context, string input)
        {
            return await _votingRepository.DeleteVotingRecord(input);
        }
    }
}
