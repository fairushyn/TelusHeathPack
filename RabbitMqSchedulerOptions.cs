using Elsa.Activities.MassTransit.Options;

namespace TelusHeathPack
{
    public class RabbitMqSchedulerOptions : RabbitMqOptions
    {
        public MessageScheduleOptions MessageSchedule { get; set; }
    }
}