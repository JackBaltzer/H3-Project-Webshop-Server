using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Webshop.API.Models;
using Webshop.Domain;
using Webshop.Data;
using Webshop.API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Webshop.API.Services
{

    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        //IEnumerable<User> GetAll();
        Login GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        // private List<User> _users = new List<User>
        // {
        //      new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        // };

        private readonly AppSettings _appSettings;
        private readonly WebshopContext _context;

        public UserService(IOptions<AppSettings> appSettings, WebshopContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.Logins.Include(l => l.Role).SingleOrDefault(x => x.Email == model.Email);

            // return null if user not found
            if (user == null) return null;

            // return null, if password doesnt match hash
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password)) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        //public IEnumerable<User> GetAll()
        //{
        //    return _users;
        //}

        public Login GetById(int id)
        {
            //return _users.FirstOrDefault(x => x.Id == id);
            return _context.Logins.Include(l => l.Role).FirstOrDefault(x => x.Id == id);
        }

        // helper methods
        private string generateJwtToken(Login login)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", login.Id.ToString()),
                    new Claim("roleAccess", login.Role.RoleAccess.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
