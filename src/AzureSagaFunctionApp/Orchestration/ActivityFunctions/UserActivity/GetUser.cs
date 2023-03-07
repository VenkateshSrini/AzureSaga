using Amazon.Runtime.Internal.Util;
using AzureSaga.Domain;
using AzureSaga.Repository;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.Orchestration.ActivityFunctions.UserActivity
{
    [DurableTask(nameof(GetUser))]
    public class GetUser:TaskActivity<string,User>
    {
        private readonly IUserRepository _userRespository;
        private readonly ILogger<GetUser> _logger;
        public GetUser(IUserRepository userRespository, ILogger<GetUser> logger)
        {
            _userRespository = userRespository;
            _logger = logger;
        }

        public override async Task<User> RunAsync(TaskActivityContext context, string input)
        {
           return await _userRespository.GetUserById(input);
        }
    }
}
