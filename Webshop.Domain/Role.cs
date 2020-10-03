using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleAccess { get; set; }
    }
}
