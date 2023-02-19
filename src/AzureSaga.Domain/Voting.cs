using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Domain
{
    public class Voting
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? GameId { get;set; }
        public int? Credit { get; set; }
    }
}
