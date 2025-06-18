using System;
using System.Threading.Tasks;
using hope4life.Data;
using hope4life.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    //[ApiController]
    [Route("api/[controller]")]
    public class EmailNotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public EmailNotificationController(ApplicationDbContext db) => _db = db;

        // GET all
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Notifications.ToListAsync());

        // POST test seed (manual insert)
        [HttpPost("seed")]
        public async Task<IActionResult> Seed([FromBody] Notification n)
        {
            n.NotificationId = Guid.NewGuid();
            n.CreatedOn = n.SendOn = DateTime.UtcNow;
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();
            return Ok(n);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            _db.Notifications.Remove(n);
            await _db.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}

