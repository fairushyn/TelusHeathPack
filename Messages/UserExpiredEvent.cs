using System;
using MassTransit;

namespace TelusHeathPack.Messages
{
    public class UserExpiredEvent : CorrelatedBy<string>
    {
        public string CorrelationId  => Alias;
        public string Alias { get; set; }
    }
}