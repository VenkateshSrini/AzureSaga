using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AzureSaga.Domain;
using AzureSaga.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace AzureSagaFunctionApp.DurableEntities
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UserCreditService
    {
        [JsonProperty("userCredit")]
        public UserCredit? UserCredits { get; set; }
              
        [JsonProperty("previousCredit")]
        public int? PreviousCredit { get; private set; }
        private readonly IUserCreditRepository _userCreditRepository;
        
        public UserCreditService(IUserCreditRepository userCreditRepository ) {
            _userCreditRepository = userCreditRepository;
            
        }
        public async Task<int?> GetCreditAsync(string userID) {
            UserCredits = await _userCreditRepository.GetUserCreditAsync(userID);
            PreviousCredit = UserCredits.Credits;
            return UserCredits.Credits;
        }
        public async Task<bool> Reset()
        {
            return await _userCreditRepository.UpdateCreditAmountAsync(UserCredits.UserId, PreviousCredit.Value)
        }
        public async Task<bool>Deduct(int amount)
        {
            var finalPoints = PreviousCredit.Value-amount;
            return await _userCreditRepository.UpdateCreditAmountAsync(UserCredits.UserId, finalPoints);

        }
        [FunctionName(nameof(UserCreditService))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
       => ctx.DispatchAsync<UserCreditService>();
    }
}
