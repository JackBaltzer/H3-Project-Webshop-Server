using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string RealName { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public int CityZip { get; set; }
        public string CityName { get; set; }
        public Login Login { get; set; }

        // needed to create customer without creating a new login
        // just pass in the loginId and everything is fine
        public int LoginId { get; set; }

    }
}
