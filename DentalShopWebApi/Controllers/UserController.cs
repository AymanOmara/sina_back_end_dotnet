
using DentalShopWebApi.AllServices;
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
        private readonly Services _services;

        public UserController(db_aa382a_ibnsinadentalContext context  , Services services)
        {
            _context = context;
            this._services = services;
        }


        // Get: api/User/getAllUsers
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
              
                var users =  await _context.Users.ToListAsync();
                return Ok(users);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // POST: api/User/CreateAccount
        [HttpPost("CreateAccount")]
        public async Task<ActionResult<User>> CreateAccount([FromBody] User user)
        {
            try
            {

                if (user.Userphoto != null)
                {
                    string imageUrl = await _services.SaveImageAsync(user.Userphoto, Guid.NewGuid(), "1", "user");
                    if (imageUrl == null)
                        return BadRequest("Invalid image data");

                    user.Userphoto = imageUrl;
                }
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(user);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/User/UpdateAccountInfo/5
        [HttpPut("UpdateAccountInfo/{id}")]
        public async Task<IActionResult> UpdateAccountInfo(int id, [FromBody] User user)
        {
            try
            {


                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return NotFound("user not found");

                if (!string.IsNullOrEmpty(existingUser.Userphoto))
                {
                    var res = await _services.DeleteImageAsync(existingUser.Userphoto);
                    if (res)
                    {
                        string imageUrl = await _services.SaveImageAsync(user.Userphoto, Guid.NewGuid(), "1", "user");
                        if (imageUrl == null)
                            return BadRequest("Invalid image data");

                        existingUser.Userphoto = imageUrl;
                    }
                }

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/User/Login
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginModel login)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/User/DeleteAccount/5
        [HttpDelete("DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(user.Userphoto))
                {

                    var res = await _services.DeleteImageAsync(user.Userphoto);
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class LoginModel
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }

}

