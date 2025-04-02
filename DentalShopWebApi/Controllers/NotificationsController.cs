using DentalShopWebApi.DAL;
using DentalShopWebApi.dto;
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
            try
            {
                return await _context.Notifications
                    .Where(n => n.Userid == userId && n.Timemark == "True")
                    .OrderByDescending(n => n.Notificationid)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Notifications/InsertNotification
        [HttpPost("InsertNotification")]
        public async Task<ActionResult<Notification>> InsertNotification([FromBody] Notification notification)
        {
            try
            {
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Notifications/MarkAsRead/5
        [HttpPut("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("RegisterFCMToken")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterUserFcm(RegisterFcmToken request)
        {
            return Ok(new BaseResponse<bool>
            {
                Data = true,
                Message = "OK",
                Success = true,
            });
        }
    }

    public record RegisterFcmToken(string FcmToken, int UserId);
}