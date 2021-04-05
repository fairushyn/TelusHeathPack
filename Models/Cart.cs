using System;
using System.Collections.Generic;

namespace TelusHeathPack.Models
{
    public class Cart
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public IList<CartItem> Items { get; set; }
    }
}
