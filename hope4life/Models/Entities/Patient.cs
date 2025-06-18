
using System.ComponentModel.DataAnnotations;

namespace hope4life.Models.Entities
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string GuardianName { get; set; } = string.Empty;

        public string Relationship { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string BloodType { get; set; } = string.Empty;

        /// <summary>
        /// Frequency of required blood in weeks (2 or 4 only).
        /// </summary>
        [Range(1, 52)]
        public int Frequency { get; set; }  // ✅ Changed


        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }
    }
}
/*using System;
using System.ComponentModel.DataAnnotations;

namespace hope4life.Models.Entities   // ⬅ DbContext یہی namespace استعمال کرے گا
{
    public class Patient
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string GuardianName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string BloodType { get; set; } = string.Empty;

        /// <summary>
        /// 2 = ہر دو ہفتے، 4 = ہر چار ہفتے۔
        /// </summary>
        [Required]
        [Range(1, 52, ErrorMessage = "FrequencyInWeeks must be 1‑52.")]
        public int FrequencyInWeeks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}*/


