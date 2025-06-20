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
