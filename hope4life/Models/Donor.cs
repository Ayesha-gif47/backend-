namespace BloodBankBackend.Models
{
    public class Donor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LastDonationDate { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}