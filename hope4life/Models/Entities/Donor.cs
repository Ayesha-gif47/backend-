using System.ComponentModel.DataAnnotations;

namespace hope4life.Models.Entities
{
    public class Donor
    {
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string BloodGroup { get; set; } = string.Empty;

        public DateTime? NextDonationDate { get; set; }

        public DateTime? LastDonationDate { get; set; }

        public bool IsBackupDonor { get; set; } = false;

        public bool IsAvailable { get; set; } = true;

        public int NotAvailableCount { get; set; } = 0;

        public bool IsBlocked { get; set; } = false;

        public string? Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }
    }
}
