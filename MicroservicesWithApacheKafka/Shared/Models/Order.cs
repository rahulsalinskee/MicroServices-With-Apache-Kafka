using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; } = 0;
    }
}
