using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;

using Microsoft.Extensions.Logging;


namespace TelusHeathPack.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Send a user data to SSI", Icon = "fas fa-user-check",
        Outcomes = new[] {OutcomeNames.Done, "Not Found"})]
    public class SendDataToSSI : Activity
    {
        private readonly ILogger<SendDataToSSI> _logger;


        public SendDataToSSI(ILogger<SendDataToSSI> logger)
        {
            _logger = logger;
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the alias of the user.")]
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }


        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context,
            CancellationToken cancellationToken)
        {
            // Retrieves user from db
            var alias = await context.EvaluateAsync(Alias, cancellationToken);

            try
            {
                // TODO: send data to the SSI
                _logger.LogInformation($"The data by user's alias: {Alias} was sent.");
                return Done();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The data by user's alias: {Alias} wasn't received");
                return Outcome("Not found");
            }
        }
    }
}