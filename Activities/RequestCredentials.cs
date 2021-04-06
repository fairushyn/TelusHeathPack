using System.Threading;
using System.Threading.Tasks;
using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using TelusHeathPack.Models;

namespace TelusHeathPack.Activities
{
    public class RequestCredentials : Activity
    {
        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;

        public RequestCredentials(IWorkflowExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }
        
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }
        
        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var ttt = Output;
            var name = await _expressionEvaluator.EvaluateAsync(Alias, context, cancellationToken);
            var person = new UserInfo { FullName = name};

            Output.SetVariable("Person", person);
            Output.SetVariable("FullName", person.FullName);

            return Done();
        }
    }
}