using DentalShopWebApi.DAL;
using DentalShopWebApi.dto;
using DentalShopWebApi.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;

namespace DentalShopWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly db_aa382a_ibnsinadentalContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NotificationsController(db_aa382a_ibnsinadentalContext context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;
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

                // 1. Get FCM token of the user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == notification.Userid);
                if (user?.FCMToken != null)
                {
                    // 2. Send notification
                    //await SendPushNotification(user.FCMToken, "New Notification", notification.Notificationtext);
                      await SendFcmV1Notification(user.FCMToken, "New Notification", notification.Notificationtext);
                }

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
            var user = await _context.Users.FindAsync(request.UserId);
            if (user != null)
            {
                user.FCMToken = request.FcmToken;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return Ok(new BaseResponse<bool>
            {
                Data = true,
                Message = "FCM token saved successfully.",
                Success = true,
            });
        }


        //private async Task<bool> SendPushNotification(string fcmToken, string title, string body)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=YOUR_SERVER_KEY_HERE");
        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

        //        var data = new
        //        {
        //            to = fcmToken,
        //            notification = new
        //            {
        //                title = title,
        //                body = body,
        //                sound = "default"
        //            },
        //            priority = "high"
        //        };

        //        var json = System.Text.Json.JsonSerializer.Serialize(data);
        //        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //        var response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);
        //        return response.IsSuccessStatusCode;
        //    }
        //}


        private async Task<string> GetAccessTokenAsync()
        {
            GoogleCredential credential;
            using (var stream = new FileStream(
                    Path.Combine(_webHostEnvironment.WebRootPath, "ibn-sina-dent-f1df955b5bcd.json"),
                    FileMode.Open,
                    FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            }

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }


        private async Task<bool> SendFcmV1Notification(string fcmToken, string title, string body)
        {
            var accessToken = await GetAccessTokenAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var message = new
                {
                    message = new
                    {
                        token = fcmToken,
                        notification = new
                        {
                            title = title,
                            body = body
                        }
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Replace YOUR_PROJECT_ID with your actual Firebase project ID
                var response = await client.PostAsync($"https://fcm.googleapis.com/v1/projects/ibn-sina-dent/messages:send", content);
                return response.IsSuccessStatusCode;
            }
        }

    }
    


    public record RegisterFcmToken(string FcmToken, int UserId);
}