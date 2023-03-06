using AzureSaga.Repository;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.VotingActivities
{
    [DurableTask(nameof(ResetVotingSaga))]
    public class ResetVotingSaga:TaskActivity<string,bool>
    {
        public readonly IVotingRepository _votingRepository;
        public readonly ILogger<AddVote> _logger;
        public ResetVotingSaga(IVotingRepository votingRepository, ILogger<AddVote> logger)
        {
            _votingRepository = votingRepository;
            _logger = logger;
        }

        public override async Task<bool> RunAsync(TaskActivityContext context, string input)
        {
            var voting = await _votingRepository.GetVotingByIdAsync(input);
            if (voting != null)
            {
                if (voting.SagaId.IsNullOrWhiteSpace())
                {
                    var updateResult = _votingRepository.UpdateSagaId(input, "");
                }

            }
            return false;
        }
    }
}
