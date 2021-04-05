using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using TelusHeathPack.Extensions;
using TelusHeathPack.Messages;
using TelusHeathPack.Models;

namespace TelusHeathPack.Controllers
{
    public class Users : IUsers
    {
        private readonly IDistributedCache _redisCache;
        private readonly ISendEndpointProvider _sender;

        public Users(ISendEndpointProvider sender, IDistributedCache redisCache)
        {
            _sender = sender;
            _redisCache = redisCache;
        }

        public async Task<User> Get(string alias)
        {
            var user = await _redisCache.GetRecordAsync<User>(alias);
            return user;
        }

        public async Task<User> Add(AddUser model)
        {
            var user = await _redisCache.GetRecordAsync<User>(model.Alias);
            if (user == null)
            {
                user = new User
                {
                    Alias = model.Alias,
                    TestNumber = GenerateNumber()
                };

                await _redisCache.SetRecordAsync(user.Alias, user); 
             
                await _sender.Send(new UserCreated
                {
                    Alias = user.Alias,
                    TestNumber = user.TestNumber,
                    Timestamp = DateTime.UtcNow
                });
            }
            
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
        
        private int GenerateNumber()
        {
            var ran = new Random(DateTime.Now.Millisecond);
            var keys = new List<int>();
            var key = 0;
            
            do
            {
                key = ran.Next(1000000000, int.MaxValue);

            } while (keys.Contains(key));

            keys.Add(key);
            return key;
        }
    }
}