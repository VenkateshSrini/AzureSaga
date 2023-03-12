using AzureSaga.Domain;
using AzureSaga.Repository;
using AzureSagaFunctionApp.Orchestration.ActivityFunctions.VotingActivities;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.SagaActivity
{
    [DurableTask(nameof(InsertSaga))]
    public class InsertSaga:TaskActivity<Saga,Saga>
    {
        public readonly ISagaRepository _sagaRepository;
        public readonly ILogger<InsertSaga> _logger;
        public InsertSaga(ISagaRepository sagaRepository, ILogger<InsertSaga> logger)
        {
            _sagaRepository = sagaRepository;
            _logger = logger;
        }

        public override async Task<Saga> RunAsync(TaskActivityContext context, Saga saga)
        {
            saga.Id = context.InstanceId;
            saga.State = SagaState.InProgress;
            return await _sagaRepository.InsertSagaAsync(saga);
        }
    }
}
