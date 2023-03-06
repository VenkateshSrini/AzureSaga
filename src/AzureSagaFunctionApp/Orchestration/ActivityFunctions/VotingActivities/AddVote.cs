using Amazon.Runtime.Internal.Util;
using AzureSaga.Domain;
using AzureSaga.Repository;
using AzureSagaFunctionApp.Orchestration.ActivityFunctions.UserCreditActivity;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.VotingActivities
{
    [DurableTask(nameof(AddVote))]
    public class AddVote:TaskActivity<Voting,Voting>
    {
        public readonly IVotingRepository _votingRepository;
        public readonly ILogger<AddVote> _logger;
        public AddVote(IVotingRepository votingRepository, ILogger<AddVote> logger)
        {
            _votingRepository = votingRepository;
            _logger = logger;
        }

        public override async Task<Voting> RunAsync(TaskActivityContext context, Voting input)
        {
            input.SagaId = context.InstanceId;
            input.RecordState = VotingRecordState.InProgress;
            return await _votingRepository.InsertVotingAsync(input);

        }
    }
}
