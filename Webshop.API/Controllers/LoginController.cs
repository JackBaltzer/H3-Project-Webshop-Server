using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Webshop.API.Models;
using Webshop.API.Services;
using Webshop.Data;
using Webshop.Domain;
using BC = BCrypt.Net.BCrypt;

namespace Webshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly WebshopContext _context;
        private readonly string _tokenKey = "LndeQioTxzvR65FaEfXr0qm9m4H822jhAN4sl8z6cZhUkkawgt371MopObySI37u";
        private readonly IUserService _userService;

        public LoginController(WebshopContext context, IUserService userService) 
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Login
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            // include roles on all logins when fetching
            return await _context.Logins.Include(l => l.Role).ToListAsync();
        }

        // GET: api/Login/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Login>> GetLogin(int id)
        {
            // get the role of the requested login
            var login = await _context.Logins.Include(l => l.Role).FirstOrDefaultAsync(l => l.Id == id);

            if (login == null)
            {
                return NotFound();
            }

            return login;
        }

        // PUT: api/Login/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogin(int id, Login login)
        {
            if (id != login.Id)
            {
                return BadRequest();
            }

            // prevent updateing a Login and removing the attached role
            if(login.Role == null && login.RoleId < 1)
            {
                return BadRequest();
            }

            _context.Entry(login).State = EntityState.Modified;


            if(login.Password != null)
            {
                login.Password = BC.HashPassword(login.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Login>> PostLogin(Login login)
        {
            // enforce a login to customer role if no role submitted
            if (login.Role == null && login.RoleId < 1)
            {
                Role role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleAccess == 1);
                login.RoleId = role.Id;
            }

            login.Password = BC.HashPassword(login.Password);

            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogin", new { id = login.Id }, login);
        }

        // DELETE: api/Login/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Login>> DeleteLogin(int id)
        {
            var login = await _context.Logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }

            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return login;
        }

        private bool LoginExists(int id)
        {
            return _context.Logins.Any(e => e.Id == id);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {


            var response = _userService.Authenticate(model, ipAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

           // setTokenCookie(response.RefreshToken);

            return Ok(response);
        }



        // Token generation happens during login 
        public string GenerateJwtToken(Login login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { 
                    new Claim("id", login.Id.ToString()),
                    new Claim("roleAccess", login.Role.RoleAccess.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var loginId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return account id from JWT token if validation successful
                return loginId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
