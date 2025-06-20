using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hope4life.Models.Entities
{
    public class DonorAssignment
    {
        [Key] public int Id { get; set; }

        [Required] public Guid PatientId { get; set; }
        [Required] public Guid DonorId { get; set; }

        [Required] public DateTime ScheduleDate { get; set; }

        public bool Donated { get; set; } = false;
        public DateTime? DonationDate { get; set; }   // set when donation done

        public bool IsWillingToDonate { get; set; } = true;

        /// <summary>"2-Week" or "4-Week"</summary>
        [Required] public string DonorType { get; set; } = "2-Week";

        /// <summary>Custom workflow states (optional)</summary>
        public int StatusId { get; set; } = 0;

        public string? Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }

        // navigation – optional
        [ForeignKey(nameof(PatientId))] public Patient? Patient { get; set; }
        [ForeignKey(nameof(DonorId))] public Donor? Donor { get; set; }
    }
}
