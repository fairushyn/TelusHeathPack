using System;
using MassTransit;

namespace TelusHeathPack.Messages
{
    public class UserCredentialsAdded : CorrelatedBy<string>
    {
        public string CorrelationId => Alias;
        public string Alias { get; set; }
        public int TestNumber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
