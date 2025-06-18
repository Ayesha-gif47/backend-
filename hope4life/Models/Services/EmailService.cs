/*using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace hope4life.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "HOPE4LIFE@gmail.com";
        private readonly string _appPassword = "sdjylelsdvltdurb";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromEmail, _appPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}*/
/*using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace hope4life.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "HOPE4LIFE@gmail.com";  // ← sender Gmail
        private readonly string _appPassword = "sdjylelsdvltdurb";     // ← 16‑char App‑Password

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // TEMP log line – remove when done
            Console.WriteLine($"FROM={"HOPE4LIFE@gmail.com"}  PWLen={16}");

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromEmail, _appPassword)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await smtp.SendMailAsync(mail);
        }
    }
}*/
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace hope4life.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hope4Life App", "HOPE4LIFE@gmail.com"));
            message.To.Add(new MailboxAddress("Donor", "hadiyamano999@gmail.com"));
            message.Subject = subject;

            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("HOPE4LIFE@gmail.com", "krdixkqycaobhzt");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

