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
    [DurableTask(nameof(GetSagaById))]
    public class GetSagaById:TaskActivity<string, Saga>
    {
        public readonly ISagaRepository _sagaRepository;
        public readonly ILogger<GetSagaById> _logger;
        public GetSagaById(ISagaRepository sagaRepository, ILogger<GetSagaById> logger)
        {
            _sagaRepository = sagaRepository;
            _logger = logger;
        }

        public override async Task<Saga> RunAsync(TaskActivityContext context, string input)
        {
            return await _sagaRepository.GetSagaByIdAsync(input);
        }
    }
}
