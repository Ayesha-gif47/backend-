using hope4life.Data;
using hope4life.Models.Entities;
using Microsoft.EntityFrameworkCore;
using hope4life.Services;

namespace hope4life.Jobs;

public interface INotificationJob
{
    Task InsertMonthlyCycleAsync();   // 1‑Method
    Task InsertDayBasedAsync();       // 2‑days‑before + same‑day
    Task ProcessPendingAsync();       // send + flag
}

public class NotificationJob : INotificationJob
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _mail;

    public NotificationJob(ApplicationDbContext db, IEmailService mail)
    {
        _db = db; _mail = mail;
    }

    // ① 1‑taareekh: har donor‑patient pair ko ek cycle message add karo
    public async Task InsertMonthlyCycleAsync()
    {
        var month = DateTime.UtcNow.Month;
        var year = DateTime.UtcNow.Year;

        var pairs = await _db.DonorAssignments
                      .Select(a => new { a.DonorId, a.PatientId })
                      .Distinct()
                      .ToListAsync();

        foreach (var p in pairs)
        {
            bool already =
                await _db.Notifications.AnyAsync(n =>
                    n.DonorId == p.DonorId &&
                    n.PatientId == p.PatientId &&
                    n.MessageType == "CycleStart" &&
                    n.CreatedOn.Month == month &&
                    n.CreatedOn.Year == year);

            if (!already)
            {
                _db.Notifications.Add(new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    DonorId = p.DonorId,
                    PatientId = p.PatientId,
                    MessageType = "CycleStart",
                    Message = "Your donation cycle starts this month.",
                    IsEmailSend = false,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }
        await _db.SaveChangesAsync();
    }

    // ② Daily: 2‑days‑before & same‑day rows insert
    public async Task InsertDayBasedAsync()
    {
        DateTime today = DateTime.UtcNow.Date;
        DateTime twoDays = today.AddDays(2);

        await AddReminderRows(twoDays, "TwoDaysBefore",
            "Your scheduled donation is in 2 days ({0:dd-MMM-yyyy}).");

        await AddReminderRows(today, "OnDonationDay",
            "Today is your scheduled donation day ({0:dd-MMM-yyyy}).");

        await _db.SaveChangesAsync();
    }

    // ③ Har 15‑min pending notifications bhejna
    public async Task ProcessPendingAsync()
    {
        var list = await _db.Notifications
                            .Where(n => !n.IsEmailSend)
                            .ToListAsync();

        foreach (var n in list)
        {
            await SendEmailPairAsync(n);
            n.IsEmailSend = true;
            n.SendOn = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
    }
    private async Task AddReminderRows(DateTime date, string type, string template)
    {
        var assigns = await _db.DonorAssignments
                               .Where(a => a.DonationDate.HasValue &&
                                           a.DonationDate.Value.Date == date.Date)
                               .ToListAsync();

        foreach (var a in assigns)
        {
            bool exists = await _db.Notifications.AnyAsync(n =>
                n.DonorId == a.DonorId &&
                n.PatientId == a.PatientId &&
                n.MessageType == type);

            if (!exists)
            {
                _db.Notifications.Add(new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    DonorId = a.DonorId,
                    PatientId = a.PatientId,
                    MessageType = type,
                    Message = string.Format(template, a.DonationDate.Value),
                    IsEmailSend = false,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }
    }


    private async Task SendEmailPairAsync(Notification n)
    {
        if (n.DonorId is not null)
        {
            var donor = await _db.Donors.FindAsync(n.DonorId);
            if (donor?.Email is not null)
                await _mail.SendEmailAsync(donor.Email, n.MessageType, n.Message);
        }
        if (n.PatientId is not null)
        {
            var pat = await _db.Patients.FindAsync(n.PatientId);
            if (pat?.Email is not null)
                await _mail.SendEmailAsync(pat.Email, n.MessageType, n.Message);
        }
    }
}

