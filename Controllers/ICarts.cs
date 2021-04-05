using System;
using System.Threading.Tasks;
using TelusHeathPack.Models;

namespace TelusHeathPack.Controllers
{
    public interface ICarts
    {
        Task<Cart> Cart(string username);
        Task<Cart> AddItem(string username, CartItem item);
        Task Submit(string username);
        void Remove(Guid cartId);
    }
}