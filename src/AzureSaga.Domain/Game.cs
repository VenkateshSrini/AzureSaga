using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Domain
{
    public class Game
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Details { get; set; }
        public int? MinCreditPoints { get; set; }
    }
}
