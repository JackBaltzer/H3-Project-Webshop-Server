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
        public Customer Customer { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderLine> OrderLines { get; set; }

    }
}
