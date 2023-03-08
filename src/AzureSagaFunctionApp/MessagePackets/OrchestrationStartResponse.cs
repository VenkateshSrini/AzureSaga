using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.MessagePackets
{
    public class OrchestrationStartResponse
    {
        [QueueOutput("saga-queue", Connection = "queue-con")]
        public UserInputRequestMessage QueueInput { get; set; }
        public StandardResponse Response { get; set; }  
    }
}
