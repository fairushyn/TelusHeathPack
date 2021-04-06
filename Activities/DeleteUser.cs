using System;
using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace TelusHeathPack.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Delete a User", Icon = "fas fa-user-minus", Outcomes = new[] { OutcomeNames.Done, "Not Found" })]
    public class DeleteUser : Activity
    {
        private readonly ILogger<DeleteUser> _logger;
        private readonly IDistributedCache _redisCache;

        public DeleteUser(ILogger<DeleteUser> logger, IDistributedCache redisCache)
        {
            _logger = logger;
            _redisCache = redisCache;
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the ID of the user to activate.")]
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            // Removes user from db
            var alias = await context.EvaluateAsync(Alias, cancellationToken);

            try
            {
                await _redisCache.RemoveAsync(alias, cancellationToken);
                _logger.LogError($"User {alias} could not be deleted");
                return Outcome("Not found");
            }
            catch (Exception e)
            {
                _logger.LogInformation($"User {alias} succesfully deleted");
                return Done();
            }
        }
    }
}