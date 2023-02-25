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
        RolledBac=2
    }
    public class Saga
    {
        public string Id { get; set; }
        public SagaState State { get; set; }
    }
}
