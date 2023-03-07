using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.MessagePackets
{
    public class StandardResponse
    {
        public int OperationStatus { get; set; }
        public string Status { get; set; }
        public string SagaId { get;set; }
    }
}
