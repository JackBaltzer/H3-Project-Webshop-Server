using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class OrderLine
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}
