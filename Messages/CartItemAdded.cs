using System;
using MassTransit;

namespace TelusHeathPack.Messages
{
    public class CartItemAdded : CorrelatedBy<Guid>
    {
        public Guid CorrelationId => CartId;

        public Guid CartId { get; set; }

        public string UserName { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
