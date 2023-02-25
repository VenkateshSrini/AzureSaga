using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Domain
{
    public class UserCredit
    {
        public string ? Id { get;set; }
        public string? UserId { get; set; }
        public int ? Credits { get; set; } = 0;
        public string? SagaId { get; set; }

    }
}
