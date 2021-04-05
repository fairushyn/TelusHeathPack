using System;
using Elsa.Activities;
using Elsa.Activities.ControlFlow.Activities;
using Elsa.Activities.MassTransit.Activities;
using Elsa.Scripting.JavaScript;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using NodaTime;
using TelusHeathPack.Messages;

namespace TelusHeathPack.Workflows
{
    public class UserTrackingWorkflow : IWorkflow
    {
        private readonly RabbitMqSchedulerOptions _options;

        public UserTrackingWorkflow(IOptions<RabbitMqSchedulerOptions> options)
        {
            _options = options.Value;
        }

        public void Build(IWorkflowBuilder builder)
        {
            builder.StartWith<ReceiveMassTransitMessage>(activity => activity.MessageType = typeof(UserCreated))
                .Then<SetVariable>(activity =>
                {
                    activity.VariableName = "LastUpdateTimestamp";
                    activity.ValueExpression = new JavaScriptExpression<Instant>("instantFromDateTimeUtc(lastResult().Timestamp)");
                })
                .Then<SetVariable>(activity =>
                {
                    activity.VariableName = "IsExpired";
                    activity.ValueExpression = new JavaScriptExpression<bool>("false");
                })
                .Fork(
                    action => action.Branches = new [] {"Item-Added", "User-Expired"},
                    fork =>
                    {
                        fork.When("Item-Added")
                            .Then<ScheduleSendMassTransitMessage>(
                                activity =>
                                {
                                    activity.MessageType = typeof(UserExpiredEvent);
                                    activity.EndpointAddress = new Uri($"{_options.Host}/user_state");
                                    activity.ScheduledTime = new JavaScriptExpression<DateTime>("plus(LastUpdateTimestamp, durationFromSeconds(10)).ToDateTimeUtc()");
                                    activity.Message = new JavaScriptExpression<UserExpiredEvent>("return { correlationId: correlationId(), alias: correlationId() }");
                                }).WithName("ScheduleExpire")
                            .Then<SetVariable>(activity =>
                            {
                                activity.VariableName = "ScheduleTokenId";
                                activity.ValueExpression = new JavaScriptExpression<bool>("lastResult()");
                            })
                            .Then<ReceiveMassTransitMessage>(activity => activity.MessageType = typeof(UserCredentialsAdded))
                            .Then<SetVariable>(activity =>
                            {
                                activity.VariableName = "LastUpdateTimestamp";
                                activity.ValueExpression = new JavaScriptExpression<Instant>("instantFromDateTimeUtc(lastResult().Timestamp)");
                            })
                            .Then<CancelScheduledMassTransitMessage>(activity => activity.TokenId = new JavaScriptExpression<Guid>("return ScheduleTokenId"))
                            .Then("ScheduleExpire");

                        fork.When("User-Expired")
                            .Then<ReceiveMassTransitMessage>(activity => activity.MessageType = typeof(UserExpiredEvent))
                            .Then<SetVariable>(activity =>
                            {
                                activity.VariableName = "IsExpired";
                                activity.ValueExpression = new JavaScriptExpression<bool>("true");
                            })
                            .Then<SendMassTransitMessage>(activity =>
                            {
                                activity.Message = new JavaScriptExpression<UserRemovedEvent>("return { alias: correlationId() };");
                                activity.EndpointAddress = new Uri($"{_options.Host}/user_service");
                                activity.MessageType = typeof(UserRemovedEvent);
                            })
                            .Then("Join");

                        // fork.When("OrderSubmitted")
                        //     .Then<ReceiveMassTransitMessage>(activity => activity.MessageType = typeof(OrderSubmitted))
                        //     .Then<SetVariable>(activity =>
                        //     {
                        //         activity.VariableName = "LastUpdateTimestamp";
                        //         activity.ValueExpression = new JavaScriptExpression<DateTime>("lastResult().Timestamp");
                        //     })
                        //     .Then<CancelScheduledMassTransitMessage>(activity => activity.TokenId = new JavaScriptExpression<Guid>("return ScheduleTokenId"))
                        //     .Then("Join");
                    })
                .Join(x => x.Mode = Join.JoinMode.WaitAny).WithName("Join");
        }
    }
}