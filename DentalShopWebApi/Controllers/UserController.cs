
using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly db_aa382a_ibnsinadentalContext _context;

        public UserController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // POST: api/User/CreateAccount
        [HttpPost("CreateAccount")]
        public async Task<ActionResult<User>> CreateAccount([FromBody] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // PUT: api/User/UpdateAccountInfo/5
        [HttpPut("UpdateAccountInfo/{id}")]
        public async Task<IActionResult> UpdateAccountInfo(int id, [FromBody] User user)
        {
            if (id != user.Userid)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/User/Login
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginModel login)
        {
            var user = await _context.Users
                .Where(u => (u.Useremail == login.Identifier || u.Userphone == login.Identifier) && u.Userpassword == login.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // DELETE: api/User/DeleteAccount/5
        [HttpDelete("DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new {success = true});
        }
    }

    public class LoginModel
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }

}

