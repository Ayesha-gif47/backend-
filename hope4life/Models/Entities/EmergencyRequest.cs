
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hope4life.Models.Entities
{
    public class EmergencyRequest
    {
        // Primary‑key
        [Key]
        public Guid Id { get; set; }           // patient_request_id

        // ---------- Foreign‑key & navigation ----------
        [Required]
        public Guid PatientId { get; set; }    // FK → Patients.Id

        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }  // Navigation property (optional but useful)

        // ---------- Domain fields ----------
        [Required]
        public string BloodType { get; set; } = string.Empty;

        /// <summary>Date and time when blood is needed.</summary>
        public DateTime RequiredOn { get; set; }

        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// e.g. 1 = Pending, 2 = In‑Progress, 3 = Fulfilled, 4 = Cancelled.
        /// Consider replacing with an enum for clarity.
        /// </summary>
        public int StatusId { get; set; } = 1;

        // ---------- Audit fields ----------
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }
    }
}

