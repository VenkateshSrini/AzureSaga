using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSaga.Domain
{
    public enum VotingRecordState
    {
        InProgress = 0,
        Completed = 1
      
    }
    public class Voting
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? GameId { get;set; }
        public int? BettingPoint { get; set; }
        public string? SagaId
        {
            get; set;
        }
        public VotingRecordState RecordState { get; set; }
    }
}
