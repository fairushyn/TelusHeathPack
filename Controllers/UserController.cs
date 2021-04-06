using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TelusHeathPack.Business;
using TelusHeathPack.Models;
using TelusHeathPack.Services.User;

namespace TelusHeathPack.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        
        private const string Username = "did:one37:56d6048b-466c-43e9-bcfa-9fb1a0950010";
        private readonly IUsersService _usersService;
        private readonly IUserBusiness _userBusiness;
        private readonly ILogger<UserController> _logger;

        public UserController(IUsersService usersService, ILogger<UserController> logger, IUserBusiness userBusiness)
        {
            _usersService = usersService;
            _logger = logger;
            _userBusiness = userBusiness;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<User>> Post([FromBody] RegistrationModel request)
        {
            _logger.LogInformation("Registering new user...");
            _logger.LogInformation($"alias: {request.Alias}");
            try
            {
                // Calls business that triggers workflow start signal execution
                // await _userBusiness.UserRegistration(request);
                // return StatusCode(StatusCodes.Status200OK);
                var result = await _usersService.Add(request);
                return new JsonResult(result);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling user registration for user {request.Alias}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #region GET: users/get
        
        // GET: api/User
        [HttpGet("get")]
        public async Task<User> Get()
        {
            _logger.LogInformation("Registering new user...");
            _logger.LogInformation($"alias: {Username}");
            
            var user = await _usersService.Get(Username);
            return user ?? new User
            {
                Alias = Guid.Empty.ToString(),
                TestNumber = 0
            };
        }
        
        #endregion
    }
}
