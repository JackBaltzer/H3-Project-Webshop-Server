using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class Login
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public Role Role { get; set; }
    }
}
