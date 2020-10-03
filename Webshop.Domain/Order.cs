using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class Order
    {
        public Order()
        {
            OrderLines = new List<OrderLine>();
        }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        public Customer Customer { get; set; }

        // needed to create order without creating a new customer
        // just pass in the customerId and everything is fine
        public int CustomerId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        // needed to create order without creating a new orderStatus
        // just pass in the orderStatusId and everything is fine
        public int OrderStatusId { get; set; }

    }
}
