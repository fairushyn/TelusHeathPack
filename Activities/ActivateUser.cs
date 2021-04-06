using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using TelusHeathPack.Extensions;
using TelusHeathPack.Models;

namespace TelusHeathPack.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Activate a User", Icon = "fas fa-user-check", Outcomes = new[] { OutcomeNames.Done, "Not Found" })]
    public class ActivateUser : Activity
    {
        private readonly IDistributedCache _redisCache;
        private readonly IWorkflowInvoker _workflowInvoker;

        public ActivateUser(IDistributedCache redisCache, IWorkflowInvoker workflowInvoker)
        {
            _redisCache = redisCache;
            _workflowInvoker = workflowInvoker;
        }
        
        [ActivityProperty(Hint = "Enter an expression that evaluates to the alias of the user to create.")]
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            // Sets the variables that will be passed to the workflow
            // var input = new Variables();
            // input.SetVariable("Alias", Alias);
            //
            // // Triggers the workflow execution (same name as registered in the Dashboard)
            // await _workflowInvoker.TriggerSignalAsync("CreatePerson", input);
            
            
            // Retrieves user from db
            var recordKey  = await context.EvaluateAsync(Alias, cancellationToken);
            var user = await _redisCache.GetRecordAsync<User>(recordKey);
            
            if (user == null)
            {
                return Outcome("Not Found");
            }

            // Updates user info on redirs 
            user.IsApproved = true; 
            await _redisCache.SetRecordAsync(recordKey, user);
            return Done();
        }
    }
}