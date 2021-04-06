using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using TelusHeathPack.Extensions;
using TelusHeathPack.Messages;
using TelusHeathPack.Models;

namespace TelusHeathPack.Services.User
{
    public class UsersServiceService : IUsersService
    {
        private readonly IDistributedCache _redisCache;
        private readonly ISendEndpointProvider _sender;

        public UsersServiceService(ISendEndpointProvider sender, IDistributedCache redisCache)
        {
            _sender = sender;
            _redisCache = redisCache;
        }

        public async Task<Models.User> Get(string alias)
        {
            var user = await _redisCache.GetRecordAsync<Models.User>(alias);
            return user;
        }

        public async Task<Models.User> Add(RegistrationModel model)
        {
            var user = await _redisCache.GetRecordAsync<Models.User>(model.Alias);
            if (user != null) return user;
            
            user = new Models.User
            {
                Alias = model.Alias,
                TestNumber = Models.User.GenerateNumber()
            };

            await _redisCache.SetRecordAsync(user.Alias, user); 
             
            await _sender.Send(new UserCreated
            {
                Alias = user.Alias,
                TestNumber = user.TestNumber,
                Timestamp = DateTime.UtcNow
            });

            return user;
        }

        public async Task Submit(string alias)
        {
            await _redisCache.RemoveAsync(alias);
            // await _sender.Send(new OrderSubmitted
            // {
            //     OrderId = Guid.NewGuid(),
            //     Alias = alias,
            //     Timestamp = DateTime.UtcNow
            // });
        }

        public async void Remove(string alias)
        {
            await _redisCache.RemoveAsync(alias);
        }
    }
}