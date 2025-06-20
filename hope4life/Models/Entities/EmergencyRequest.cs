using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hope4life.Models.Entities
{
    public class EmergencyRequest
    {
        // Primary‑key
        [Key]
        public Guid Id { get; set; }       
        [Required]
        public Guid PatientId { get; set; }    

        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }  
        [Required]
        public string BloodType { get; set; } = string.Empty;
        public DateTime RequiredOn { get; set; }

        public string Location { get; set; } = string.Empty;
        public int StatusId { get; set; } = 1;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }
    }
}

