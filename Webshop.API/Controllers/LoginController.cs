using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public LoginController(WebshopContext context)
        {
            _context = context;
        }

        // GET: api/Login
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            // include roles on all logins when fetching
            return await _context.Logins.Include(l => l.Role).ToListAsync();
        }

        // GET: api/Login/5
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
    }
}
