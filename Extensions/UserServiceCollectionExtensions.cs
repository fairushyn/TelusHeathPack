using Microsoft.Extensions.DependencyInjection;
using TelusHeathPack.Activities;

namespace TelusHeathPack.Extensions
{
    public static class UserServiceCollectionExtensions
    {
        public static IServiceCollection AddUserActivities(this IServiceCollection services)
        {
            return services
                .AddActivity<CreateUser>()
                .AddActivity<ActivateUser>()
                .AddActivity<SendDataToSSI>() // TODO: needs implementation;
                .AddActivity<DeleteUser>();
        }
    }
}