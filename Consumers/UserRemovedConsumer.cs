using System.Threading.Tasks;
using MassTransit;
using TelusHeathPack.Controllers;
using TelusHeathPack.Messages;

namespace TelusHeathPack.Consumers
{
    public class UserRemovedConsumer : IConsumer<UserRemovedEvent>
    {
        private readonly IUsers _users;

        public UserRemovedConsumer(IUsers users)
        {
            _users = users;
        }

        public Task Consume(ConsumeContext<UserRemovedEvent> context)
        {
            if (context.Message.Alias is not null)
                _users.Remove(context.Message.Alias);
            return Task.CompletedTask;
        }
    }
}
