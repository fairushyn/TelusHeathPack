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
    public class CreatePerson : Activity
    {
        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;

        public CreatePerson(IWorkflowExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }

        public WorkflowExpression<string> TitleExpression
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        public WorkflowExpression<int> AgeExpression
        {
            get => GetState<WorkflowExpression<int>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var name = await _expressionEvaluator.EvaluateAsync(TitleExpression, context, cancellationToken);
            var age = await _expressionEvaluator.EvaluateAsync(AgeExpression, context, cancellationToken);
            var person = new Person { FullName = name, Age = age };

            Output.SetVariable("Person", person);
            Output.SetVariable("FullName", person.FullName);
            Output.SetVariable("Age", person.Age);
            
            return Done();
        }
    }
}