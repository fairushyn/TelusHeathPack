using System.Threading.Tasks;
using MassTransit;
using TelusHeathPack.Controllers;
using TelusHeathPack.Messages;

namespace TelusHeathPack.Consumers
{
    public class CartRemovedConsumer : IConsumer<CartRemovedEvent>
    {
        private readonly ICarts carts;

        public CartRemovedConsumer(ICarts carts)
        {
            this.carts = carts;
        }

        public Task Consume(ConsumeContext<CartRemovedEvent> context)
        {
            carts.Remove(context.Message.CartId);

            return Task.CompletedTask;
        }
    }
}
