using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Webshop.Domain
{
    public class Login
    {
        public int Id { get; set; }
        public string Email { get; set; }
       
        //[JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

        public Role Role { get; set; }

        // needed to create login without creating a new role
        // just pass in the roleId and everything is fine
        public int RoleId { get; set; }
    }
}
