using System.Threading.Tasks;
using MassTransit;
using TelusHeathPack.Controllers;
using TelusHeathPack.Messages;
using TelusHeathPack.Services.User;

namespace TelusHeathPack.Consumers
{
    public class UserRemovedConsumer : IConsumer<UserRemovedEvent>
    {
        private readonly IUsersService _usersService;

        public UserRemovedConsumer(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public Task Consume(ConsumeContext<UserRemovedEvent> context)
        {
            if (context.Message.Alias is not null)
                _usersService.Remove(context.Message.Alias);
            return Task.CompletedTask;
        }
    }
}
