using System;
using System.ComponentModel.DataAnnotations;

namespace hope4life.Models.Entities
{
    public class StatusMaster
    {
        [Key] public int StatusId { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = string.Empty;  // "Pending", "Completed", "Rejected"
    }
}


