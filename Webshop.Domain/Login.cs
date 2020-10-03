using System;
using System.Collections.Generic;
using System.Text;

namespace Webshop.Domain
{
    public class Login
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }

        // needed to create login without creating a new role
        // just pass in the roleId and everything is fine
        public int RoleId { get; set; }
    }
}
