using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Domain;

namespace Webshop.API.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleAccess { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(Login login, string token)
        {
            Id = login.Id;
            Email = login.Email;
            //Password = user.Password;
            RoleAccess = login.Role.RoleAccess;
            Token = token;
        }
    }
}
