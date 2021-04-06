using System.Threading.Tasks;
using TelusHeathPack.Models;

namespace TelusHeathPack.Services.User
{
    public interface IUsersService
    {
        Task<Models.User> Get(string alias);
        Task<Models.User> Add(RegistrationModel model);
        void Remove(string alias);
    }
}