using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using TelusHeathPack.Extensions;
using TelusHeathPack.Messages;
using TelusHeathPack.Models;

namespace TelusHeathPack.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Create a User", Icon = "fas fa-user-plus", Outcomes = new[] { OutcomeNames.Done })]
    public class CreateUser : Activity
    {
        private readonly ILogger<CreateUser> _logger;
        private readonly IIdGenerator _idGenerator;
        private readonly IDistributedCache _redisCache;
        private readonly ISendEndpointProvider _sender;
        
        public CreateUser(
            ILogger<CreateUser> logger,
            IIdGenerator idGenerator, IDistributedCache redisCache, ISendEndpointProvider sender)
        {
            _logger = logger;
            _idGenerator = idGenerator;
            _redisCache = redisCache;
            _sender = sender;
        }
        
        [ActivityProperty(Hint = "Enter an expression that evaluates to the alias of the user to create.")]
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }
        
        
        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            // Create and persist the new user
            var user = new User
            {
                Id = _idGenerator.Generate(),
                Alias = await context.EvaluateAsync(Alias, cancellationToken),
                TestNumber = User.GenerateNumber(),
                IsApproved = false
            };

            try
            { 
               var record = await _redisCache.GetRecordAsync<User>(user.Alias);
               if (record is null)
               {
                   await _redisCache.SetRecordAsync(user.Alias, user);
               }
               
               // await _store.InsertOneAsync(user, cancellationToken: cancellationToken);
               // Set the info that will be available through Output
            
               Output.SetVariable("User", user);
               _logger.LogInformation($"New user created: {user.Id}, {user.Alias}");
               
               await _sender.Send(new UserCreated
               {
                   Alias = user.Alias,
                   TestNumber = user.TestNumber,
                   Timestamp = DateTime.UtcNow
               }, cancellationToken: cancellationToken);
               
               return Done();
               
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Error persisting user: {user.Id}, {user.Alias}");
                return Outcome("New user not persisted");
            }
        }

       
    }
}