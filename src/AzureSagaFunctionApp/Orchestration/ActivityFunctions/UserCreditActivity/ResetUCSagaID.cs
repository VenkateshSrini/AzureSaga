using AzureSaga.Repository;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.UserCreditActivity
{
    [DurableTask(nameof(ResetUCSagaID))]
    public class ResetUCSagaID : TaskActivity<string, bool>
    {
        private readonly IUserCreditRepository _userCreditRepository;
        private readonly ILogger<UserCreditSagaIdUpdate> _logger;
        public ResetUCSagaID(IUserCreditRepository userCreditRepository,
            ILogger<UserCreditSagaIdUpdate> logger)
        {
            _userCreditRepository = userCreditRepository;
            _logger = logger;
        }
        public override async Task<bool> RunAsync(TaskActivityContext context, string input)
        {
            var userCreditObj = await _userCreditRepository.GetUserCreditAsync(input);
            if (userCreditObj != null)
            {
                if (userCreditObj.SagaId.IsNullOrWhiteSpace())
                {
                    var updateResult = _userCreditRepository.UpdateSagaId(input, "");
                }

            }
            return false;
        }
    }
}
