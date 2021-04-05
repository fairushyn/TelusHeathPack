using System;
using MassTransit;

namespace TelusHeathPack.Messages
{
    public class CartExpiredEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public Guid CartId { get; set; }
    }
}