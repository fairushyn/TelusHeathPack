using Elsa.Activities.Workflows.Extensions;
using Elsa.Models;
using Elsa.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TelusHeathPack.Models;

namespace TelusHeathPack.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly ILogger<UserBusiness> _logger;
        private readonly IWorkflowInvoker _workflowInvoker;

        public UserBusiness(
            ILogger<UserBusiness> logger,
            IWorkflowInvoker workflowInvoker)
        {
            _logger = logger;
            _workflowInvoker = workflowInvoker;
        }

        public async Task UserRegistration(RegistrationModel request)
        {
            _logger.LogInformation("Triggering RegisterUser signal...");

            // Sets the variables that will be passed to the workflow
            var input = new Variables();
            input.SetVariable("RegistrationModel", request);

            // Triggers the workflow execution (same name as registered in the Dashboard)
            await _workflowInvoker.TriggerSignalAsync("RegisterUser", input);
        }
    }
}