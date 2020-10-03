using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public Category Category { get; set; }

        // needed to create product without creating a new category
        // just pass in the categoryId and everything is fine
        public int CategoryId { get; set; }
    }
}
