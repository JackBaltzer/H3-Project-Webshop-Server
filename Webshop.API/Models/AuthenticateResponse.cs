using System.Text.Json.Serialization;
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
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(Login login, string jwtToken, string refreshToken)
        {
            Id = login.Id;
            Email = login.Email;
            RoleAccess = login.Role.RoleAccess;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
