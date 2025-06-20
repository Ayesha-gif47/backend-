using System;
using System.Threading.Tasks;
using hope4life.Data;
using hope4life.Models.Entities;
using hope4life.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailNotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _mail;

        public EmailNotificationController(ApplicationDbContext db, IEmailService mail)
        {
            _db = db;
            _mail = mail;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Notifications.ToListAsync());

        [HttpPost("seed")]
        public async Task<IActionResult> Seed([FromBody] Notification n)
        {
            n.NotificationId = Guid.NewGuid();
            n.CreatedOn = n.SendOn = DateTime.UtcNow;
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();

            await _mail.SendEmailAsync("test@example.com", n.MessageType, n.Message);
            return Ok(n);
        }
    }
}




