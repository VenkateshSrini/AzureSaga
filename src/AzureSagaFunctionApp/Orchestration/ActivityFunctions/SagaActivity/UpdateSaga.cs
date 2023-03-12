using AzureSaga.Domain;
using AzureSaga.Repository;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.SagaActivity
{
    [DurableTask(nameof(UpdateSaga))]
    public class UpdateSaga:TaskActivity< (string sagaId,SagaState sagaState),bool>
    {
        public readonly ISagaRepository _sagaRepository;
        public readonly ILogger<UpdateSaga> _logger;
        public UpdateSaga(ISagaRepository sagaRepository, ILogger<UpdateSaga> logger)
        {
            _sagaRepository = sagaRepository;
            _logger = logger;
        }

        public override async Task<bool> RunAsync(TaskActivityContext context, (string sagaId, SagaState sagaState) sagaDetails)
        {
            return await _sagaRepository.UpdateVotingRecordStatus(sagaDetails.sagaId, sagaDetails.sagaState);
        }
    }
}
