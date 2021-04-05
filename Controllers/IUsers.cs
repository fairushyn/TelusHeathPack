using System.Threading.Tasks;
using TelusHeathPack.Models;

namespace TelusHeathPack.Controllers
{
    public interface IUsers
    {
        Task<User> Get(string alias);
        Task<User> Add(AddUser model);
        Task Submit(string alias);
        void Remove(string alias);
    }
}