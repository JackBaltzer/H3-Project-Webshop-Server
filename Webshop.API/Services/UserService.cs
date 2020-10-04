using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Webshop.API.Models;
using Webshop.Domain;
using Webshop.Data;
using Webshop.API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Webshop.API.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private WebshopContext _context;
        private readonly AppSettings _appSettings;

        public UserService(
            WebshopContext context,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var login = _context.Logins.Include(l => l.Role).SingleOrDefault(x => x.Email == model.Email);

            // return null if user not found
            if (login == null) return null;

            if(!BCrypt.Net.BCrypt.Verify(model.Password, login.Password))
            {
                return null;
            }
            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(login);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token
            login.RefreshTokens.Add(refreshToken);
            _context.Update(login);
            _context.SaveChanges();

            return new AuthenticateResponse(login, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var login = _context.Logins.Include(l => l.Role).SingleOrDefault(l => l.RefreshTokens.Any(t => t.Token == token));

            // return null if no user found with token
            if (login == null) return null;

            var refreshToken = login.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            login.RefreshTokens.Add(newRefreshToken);
            _context.Update(login);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(login);

            return new AuthenticateResponse(login, jwtToken, newRefreshToken.Token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        // helper methods

        private string generateJwtToken(Login login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("LndeQioTxzvR65FaEfXr0qm9m4H822jhAN4sl8z6cZhUkkawgt371MopObySI37u");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                   new Claim("id", login.Id.ToString()),
                   new Claim(ClaimTypes.Role, login.Role.Name),
                   new Claim("roleAccess", login.Role.RoleAccess.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
