/*using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hope4life.Services;          // EmailService
using hope4life.Data;              // ApplicationDbContext
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]      // -> /api/email
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext _db;

        public EmailController(EmailService emailService, ApplicationDbContext db)
        {
            _emailService = emailService;
            _db = db;
        }

        // ───────────────────────────────────────────────
        // 1) Simple test endpoint
        //    POST /api/email/send-test
        // ───────────────────────────────────────────────
        [HttpPost("send-test")]
        public async Task<IActionResult> SendTest()
        {
            await _emailService.SendEmailAsync(
                "hadiyamano999@gmail.com",
                "dafa dor",
                "nikla chal");
            return Ok("Email Sent");
        }

        // ───────────────────────────────────────────────
        // 2) Send donor-specific reminder
        //    POST /api/email/send-donation-reminder/{donorId}
        // ───────────────────────────────────────────────
        [HttpPost("send-donation-reminder/{donorId}")]
        public async Task<IActionResult> SendDonationReminder(Guid donorId)
        {
            // 1. fetch donor from DB (needs Email column in Donors table)
            var donor = await _db.Donors
                                 .FirstOrDefaultAsync(d => d.Id == donorId); // change Id to your key
            if (donor == null || string.IsNullOrEmpty(donor.Email))
                return NotFound("Donor not found or email missing");

            // 2. compose and send email
            string subject = "Donation Reminder";
            string body = "Please remember your scheduled blood donation in 2 days.";
            await _emailService.SendEmailAsync(donor.Email, subject, body);

            return Ok($"Email sent to {donor.Email}");
        }
    }
}*/
/*using System;
using System.Threading.Tasks;
using hope4life.Data;
using hope4life.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]            // → /api/email
    public class EmailController : ControllerBase
    {
        private readonly EmailService _email;
        private readonly ApplicationDbContext _db;

        public EmailController(EmailService email, ApplicationDbContext db)
        {
            _email = email;
            _db = db;
        }

        // POST /api/email/send-test  – sends to hadiyamano999@gmail.com
        [HttpPost("send-test")]
        public async Task<IActionResult> SendTest()
        {
            await _email.SendEmailAsync(
                "hadiyamano999@gmail.com",
                "Test Subject",
                "Test Body");

            return Ok("Email Sent");
        }

        // POST /api/email/send-donation-reminder/{donorId}
        [HttpPost("send-donation-reminder/{donorId}")]
        public async Task<IActionResult> SendDonationReminder(Guid donorId)
        {
            var donor = await _db.Donors.FirstOrDefaultAsync(d => d.Id == donorId);
            if (donor == null || string.IsNullOrWhiteSpace(donor.Email))
                return NotFound("Donor not found or email missing");

            await _email.SendEmailAsync(
                donor.Email,
                "Donation Reminder",
                "Please remember your scheduled blood donation in 2 days.");

            return Ok($"Email sent to {donor.Email}");
        }
    }
}*/
using Microsoft.AspNetCore.Mvc;
using hope4life.Services;
using System.Threading.Tasks;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly EmailService _emailService;

        public NotificationController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            await _emailService.SendEmailAsync(
                "hadiyamano999@gmail.com",
                "Donation Reminder",
                "Your donation is in 2 days!");

            return Ok("Email sent successfully!");
        }
    }
}
