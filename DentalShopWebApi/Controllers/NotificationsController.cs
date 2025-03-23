using DentalShopWebApi.DAL;
using DentalShopWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;

        public NotificationsController(db_aa382a_ibnsinadentalContext context)
        {
            _context = context;
        }

        // GET: api/Notifications/GetNotifications/5
        [HttpGet("GetNotifications/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.Userid == userId && n.Timemark == "True")
                .OrderByDescending(n => n.Notificationid)
                .ToListAsync();
        }

        // POST: api/Notifications/InsertNotification
        [HttpPost("InsertNotification")]
        public async Task<ActionResult<Notification>> InsertNotification([FromBody] Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return Ok( notification);
        }

        // PUT: api/Notifications/MarkAsRead/5
        [HttpPut("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.Isread = "True";
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

