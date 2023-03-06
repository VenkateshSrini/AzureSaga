using AzureSaga.Repository;
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
    public class UpdateUserCredits: TaskActivity<(string userId,int amount),bool>
    {
        private readonly IUserCreditRepository _userCreditRepository;
        private readonly ILogger<UserCreditSagaIdUpdate> _logger;
        public UpdateUserCredits(IUserCreditRepository userCreditRepository,
            ILogger<UserCreditSagaIdUpdate> logger)
        {
            _userCreditRepository = userCreditRepository;
            _logger = logger;
        }

        public override async Task<bool> RunAsync(TaskActivityContext context, (string userId, int amount) input)
        {
            var userCreditObj = await _userCreditRepository.GetUserCreditAsync(input.userId);
            if (userCreditObj != null)
            {
                if(userCreditObj.SagaId.CompareTo(context.InstanceId)==0)
                    return await _userCreditRepository.UpdateCreditAmountAsync(input.userId,input.amount);

            }
            return false;
        }
    }
}
