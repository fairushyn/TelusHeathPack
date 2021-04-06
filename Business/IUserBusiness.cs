using System.Threading.Tasks;
using TelusHeathPack.Models;

namespace TelusHeathPack.Business
{
    public interface IUserBusiness
    {
        Task UserRegistration(RegistrationModel request);
    }
}