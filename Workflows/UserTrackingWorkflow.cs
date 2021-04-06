using Elsa.Activities;
using Elsa.Activities.ControlFlow.Activities;
using Elsa.Activities.MassTransit.Activities;
using Elsa.Expressions;
using Elsa.Scripting.JavaScript;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using NodaTime;
using TelusHeathPack.Activities;
using TelusHeathPack.Messages;
using TelusHeathPack.Models;

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
                .Then<ActivateUser>(
                    x =>
                    {
                        x.Alias =  new LiteralExpression<string>("did:one37:56d6048b-466c-43e9-bcfa-9fb1a0950010");
                    })
                .Then<RequestCredentials>(
                    x =>
                    {
                        x.Alias =  new LiteralExpression<string>("did:one37:56d6048b-466c-43e9-bcfa-9fb1a0950010");
                    })
                .Then<SetVariable>(
                    x =>
                    {
                        x.VariableName = "Person";
                        x.ValueExpression = new JavaScriptExpression<UserInfo>("CreatePerson.Person");
                    })
                .Then<SetVariable>(
                    x =>
                    {
                        x.VariableName = "Age";
                        x.ValueExpression = new JavaScriptExpression<int>("CreatePerson.Person.Age");
                    })
                // .Fork(
                //     action => action.Branches = new [] {"Item-Added", "User-Expired"},
                //     fork =>
                //     {
                //         fork.When("Item-Added")
                //             .Then<ScheduleSendMassTransitMessage>(
                //                 activity =>
                //                 {
                //                     activity.MessageType = typeof(UserExpiredEvent);
                //                     activity.EndpointAddress = new Uri($"{_options.Host}/user_state");
                //                     activity.ScheduledTime = new JavaScriptExpression<DateTime>("plus(LastUpdateTimestamp, durationFromSeconds(10)).ToDateTimeUtc()");
                //                     activity.Message = new JavaScriptExpression<UserExpiredEvent>("return { correlationId: correlationId(), alias: correlationId() }");
                //                 }).WithName("ScheduleExpire")
                //             .Then<SetVariable>(activity =>
                //             {
                //                 activity.VariableName = "ScheduleTokenId";
                //                 activity.ValueExpression = new JavaScriptExpression<bool>("lastResult()");
                //             })
                //             .Then<ReceiveMassTransitMessage>(activity => activity.MessageType = typeof(UserCredentialsAdded))
                //             .Then<SetVariable>(activity =>
                //             {
                //                 activity.VariableName = "LastUpdateTimestamp";
                //                 activity.ValueExpression = new JavaScriptExpression<Instant>("instantFromDateTimeUtc(lastResult().Timestamp)");
                //             })
                //             .Then<CancelScheduledMassTransitMessage>(activity => activity.TokenId = new JavaScriptExpression<Guid>("return ScheduleTokenId"))
                //             .Then("ScheduleExpire");
                //
                //         fork.When("User-Expired")
                //             .Then<ReceiveMassTransitMessage>(activity => activity.MessageType = typeof(UserExpiredEvent))
                //             .Then<SetVariable>(activity =>
                //             {
                //                 activity.VariableName = "IsExpired";
                //                 activity.ValueExpression = new JavaScriptExpression<bool>("true");
                //             })
                //             .Then<SendMassTransitMessage>(activity =>
                //             {
                //                 activity.Message = new JavaScriptExpression<UserRemovedEvent>("return { alias: correlationId() };");
                //                 activity.EndpointAddress = new Uri($"{_options.Host}/user_service");
                //                 activity.MessageType = typeof(UserRemovedEvent);
                //             })
                //             .Then("Join");
                //     })
                .Join(x => x.Mode = Join.JoinMode.WaitAny).WithName("Join");
        }
    }
}