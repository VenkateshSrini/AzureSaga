using AzureSaga.Domain;
using AzureSaga.Repository;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;


namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions
{
    [DurableTask(nameof(GetUserCreditInfo))]
    public class GetUserCreditInfo : TaskActivity<string, AzureSaga.Domain.UserCredit>
    {
        private readonly IUserCreditRepository _userCreditRepository;
        private readonly ILogger<UserCreditSagaIdUpdate> _logger;
        public GetUserCreditInfo(IUserCreditRepository userCreditRepository,
            ILogger<UserCreditSagaIdUpdate> logger)
        {
            _userCreditRepository = userCreditRepository;
            _logger = logger;
        }
        public override async Task<AzureSaga.Domain.UserCredit> RunAsync(TaskActivityContext context, string input)
        {
            return  await _userCreditRepository.GetUserCreditAsync(input);
        }
    }
}
