using MassTransit;

namespace TelusHeathPack.Messages
{
    public class UserRemovedEvent : CorrelatedBy<string>
    {
        public string CorrelationId => Alias;
        public string Alias { get; set; }
    }
}