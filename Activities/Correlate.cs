using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;

namespace TelusHeathPack.Activities
{
    /// <summary>
    /// Sets the CorrelationId of the workflow to a given value.
    /// </summary>
    [ActivityDefinition(
        Category = "Workflows",
        Description = "Set the CorrelationId of the workflow to a given value.",
        Icon = "fas fa-link"
    )]
    public class Correlate : Activity
    {
        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;

        public Correlate(IWorkflowExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }

        [ActivityProperty(Hint = "An expression that evaluates to the value to store as the correlation ID.")]
        public WorkflowExpression<string> ValueExpression
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(
            WorkflowExecutionContext workflowContext,
            CancellationToken cancellationToken)
        {
            var value = await _expressionEvaluator.EvaluateAsync(ValueExpression, workflowContext, cancellationToken);
            workflowContext.Workflow.CorrelationId = value;
            return Done();
        }
    }
}