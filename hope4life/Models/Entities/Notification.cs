using System;
using System.ComponentModel.DataAnnotations;

namespace hope4life.Models.Entities
{
    public class Notification
    {
        [Key] public Guid NotificationId { get; set; }

        public Guid? DonorId { get; set; }
        public Guid? PatientId { get; set; }

        [Required] public string MessageType { get; set; } = string.Empty;  // e.g. "CycleStart", "TwoDaysBefore", "OnDonationDay"
        [Required] public string Message { get; set; } = string.Empty;

        public bool IsEmailSend { get; set; } = false;
        public DateTime SendOn { get; set; } = DateTime.UtcNow;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

