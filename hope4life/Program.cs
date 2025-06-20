using hope4life.Data;
using hope4life.Models;
using hope4life.Services;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using hope4life.Jobs;     // naya job namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(cfg =>
    cfg.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddTransient<INotificationJob, NotificationJob>(); // naya job class
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();
app.UseHangfireDashboard("/hangfire");   // optional UI

// ① 1st‑of‑month cycle notification (CRON: 5 minutes past 00:00 on day 1)
RecurringJob.AddOrUpdate<INotificationJob>(
    "InsertMonthlyCycle",
    job => job.InsertMonthlyCycleAsync(),
    "0 5 1 * *");      // “0 5” = 00:05, “1” day‑of‑month

// ② Daily job to INSERT 2‑days‑before & same‑day rows (00:10 AM)
RecurringJob.AddOrUpdate<INotificationJob>(
    "InsertDailyReminders",
    job => job.InsertDayBasedAsync(),
    "10 0 * * *");     // 00:10 every day

// ③ Every 15 min job to SEND pending (IsEmailSend = false)
RecurringJob.AddOrUpdate<INotificationJob>(
    "SendPending",
    job => job.ProcessPendingAsync(),
    "*/15 * * * *");   // every 15 minutes


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
    await emailService.SendEmailAsync(
        "zainairfan11@gmail.com",         // 👉 yahan apna email daalo
        "Test Email from Program.cs",
        "<b>This is a test email sent without a controller.</b>");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
