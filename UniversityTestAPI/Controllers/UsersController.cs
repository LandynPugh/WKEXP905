using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTestAPI.Context;
using UniversityTestAPI.Models;

namespace UniversityTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WKEXP905Context _context;

        public UsersController(WKEXP905Context context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/UsersByFacultyId/5
        [HttpGet("ByFacultyId/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByFacultyId(Guid id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var users = await _context.User.Where(u => u.FacultyId == id).ToListAsync();

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }


        // GET: api/UsersByCollegeId/5
        [HttpGet("ByCollegeId/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByCollegeId(Guid id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var users = await (from x in _context.User
                         join y in _context.Faculty on x.FacultyId equals y.FacultyId
                         join z in _context.College on y.CollegeId equals z.CollegeId
                         where z.CollegeId == id
                         select x).ToListAsync();
            //var users = await _context.User.Join<Faculty, User, Guid, User>(_context.Faculty, u => Guid.Parse(u.FacultyId), f => f.FacultyId, (u, f) => new { u, f }).Where(uf => uf.f.CollegeId == id).Select(uf => uf.u).ToListAsync();
            //var users = await _context.User.Join(_context.Faculty, u => u.FacultyId, f => f.FacultyId, (u, f) => new { u, f }).Where(uf => uf.f.CollegeId == id.ToString()).Select(uf => uf.u).ToListAsync();

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // GET: 


        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.User == null)
          {
              return Problem("Entity set 'WKEXP905Context.User'  is null.");
          }
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
