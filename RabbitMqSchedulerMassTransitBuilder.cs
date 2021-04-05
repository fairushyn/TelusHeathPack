using System;
using Elsa.Activities.MassTransit.Extensions;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.QuartzIntegration;
using Quartz;
using TelusHeathPack.Consumers;
using TelusHeathPack.Messages;
using Elsa.Activities.MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TelusHeathPack
{
    internal class RabbitMqSchedulerMassTransitBuilder : MassTransitBuilderBase<RabbitMqSchedulerOptions>
    {
        private readonly IScheduler scheduler;

        public RabbitMqSchedulerMassTransitBuilder(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        protected override IBusControl CreateBus(IServiceProvider serviceProvider)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<RabbitMqSchedulerOptions>>().Value;
                var host = cfg.Host(new Uri(options.Host), h =>
                {
                    if (!string.IsNullOrEmpty(options.Username))
                    {
                        h.Username(options.Username);

                        if (!string.IsNullOrEmpty(options.Password))
                        {
                            h.Password(options.Password);
                        }
                    }
                });

                cfg.UseMessageScheduler(options.MessageSchedule.SchedulerAddress);

                // cfg.ReceiveEndpoint("user_service", ep =>
                // {
                //     ep.ConfigureConsumer<UserRemovedConsumer>(serviceProvider);
                // });

                cfg.ReceiveEndpoint("user_state", ep =>
                {
                    ep.PrefetchCount = 16;

                    ep.UseMessageRetry(r => r.Interval(2, 100));

                    // Consume all workflow messages from the same queue.
                    ep.ConfigureWorkflowConsumer<UserCreated>(serviceProvider);
                    // ep.ConfigureWorkflowConsumer<UserCredentialsAdded>(serviceProvider);
                    // ep.ConfigureWorkflowConsumer<OrderSubmitted>(serviceProvider);
                    // ep.ConfigureWorkflowConsumer<UserExpiredEvent>(serviceProvider);
                });

                // Should use external process scheduler service
                // https://github.com/MassTransit/MassTransit/tree/develop/src/Samples/MassTransit.QuartzService
                cfg.ReceiveEndpoint("quartz_scheduler", e =>
                {
                    // For MT4.0, prefetch must be set for Quartz prior to anything else
                    e.PrefetchCount = 1;
                    cfg.UseMessageScheduler(e.InputAddress);

                    e.ConfigureConsumer<ScheduleMessageConsumer>(serviceProvider);
                    e.ConfigureConsumer<CancelScheduledMessageConsumer>(serviceProvider);
                });
            });

            scheduler.JobFactory = new MassTransitJobFactory(bus);

            return bus;
        }

        protected override void ConfigureMassTransit(IServiceCollectionConfigurator configurator)
        {
            // Configure scheduler service consumers.
            configurator.AddConsumer<ScheduleMessageConsumer>();
            configurator.AddConsumer<CancelScheduledMessageConsumer>();

            // configure workflow consumers
            configurator.AddWorkflowConsumer<UserCreated>();
            configurator.AddWorkflowConsumer<UserCredentialsAdded>();
           // configurator.AddWorkflowConsumer<OrderSubmitted>();
            configurator.AddWorkflowConsumer<UserExpiredEvent>();

            // host fake service consumers
            // configurator.AddConsumer<UserRemovedConsumer>();
        }
    }
}