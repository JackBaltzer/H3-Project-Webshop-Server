using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class OrderLine
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public Order Order { get; set; }

        // needed to create orderLine without creating a new order
        // just pass in the orderId and everything is fine
        public int OrderId { get; set; }

        public Product Product { get; set; }

        // needed to create orderLine without creating a new product
        // just pass in the productId and everything is fine
        public int ProductId { get; set; }
    }
}
