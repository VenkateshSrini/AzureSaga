using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSagaFunctionApp.MessagePackets
{
    public class UserInputRequestMessage
    {
       public string GameId { get; set; }
        public string UserId { get; set; }
    }
}
