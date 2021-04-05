using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TelusHeathPack.Models;

namespace TelusHeathPack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string Username = "did:one37:56d6048b-466c-43e9-bcfa-9fb1a0950010";
        private readonly IUsers _users;

        public UserController(IUsers users)
        {
            _users = users;
        }

        // GET: api/User
        [HttpGet]
        [Route("get")]
        public async Task<User> User()
        {
            var user = await _users.Get(Username);
            return user ?? new User
            {
                Alias = Guid.Empty.ToString(),
                TestNumber = 0
            };
        }

        // POST: api/User
        [HttpPost]
        [Route("add")]
        public async Task<User> Post([FromBody] AddUser model)
        {
            return await _users.Add(model);
        }

        // POST: api/user/submit
        [HttpPost]
        [Route("submit")]
        public async Task SubmitOrder()
        {
            await _users.Submit(Username);
        }
    }
}
