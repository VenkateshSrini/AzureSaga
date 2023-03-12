using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Domain
{
    public enum SagaState
    {
        InProgress = 0,
        Completed = 1,
        RolledBack=2
    }
    public class Saga
    {
        public string UserId { get;set; }
        public string Id { get; set; }
        public SagaState State { get; set; }
        public string Message { get; set; }
    }
}
