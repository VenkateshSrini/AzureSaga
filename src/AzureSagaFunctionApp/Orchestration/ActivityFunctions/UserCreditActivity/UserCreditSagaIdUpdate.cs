using AzureSaga.Repository;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions
{
    [DurableTask(nameof(UserCreditSagaIdUpdate))]
    public class UserCreditSagaIdUpdate : Microsoft.DurableTask.TaskActivity<string, bool>
    {
        private readonly IUserCreditRepository _userCreditRepository;
        private readonly ILogger<UserCreditSagaIdUpdate> _logger;
        public UserCreditSagaIdUpdate(IUserCreditRepository userCreditRepository, 
            ILogger<UserCreditSagaIdUpdate> logger)
        {
            _userCreditRepository = userCreditRepository;
            _logger = logger;
        }

        public override async Task<bool> RunAsync(TaskActivityContext context, string input)
        {

            try
            {
                var userCreditObj = await _userCreditRepository.GetUserCreditAsync(input);
                if (userCreditObj != null)
                {
                    if (!userCreditObj.SagaId.IsNullOrWhiteSpace())
                    {
                        var updateResult = _userCreditRepository.UpdateSagaId(input, context.InstanceId);
                    }

                }
                return false;
            }
            catch
            {
                return false;
            }
            
        }

       
    }
}
