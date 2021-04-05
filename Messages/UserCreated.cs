using System;
using MassTransit;

namespace TelusHeathPack.Messages
{
    public class UserCreated : CorrelatedBy<string>
    {
        public string CorrelationId => Alias;
        public int TestNumber { get; set; }
        public string Alias { get; set; }
        public DateTime Timestamp { get; set; }
      
    }
}