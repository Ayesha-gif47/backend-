namespace BloodBankBackend.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime LastDonationDate { get; set; }
        public string DonationCycle { get; set; } = string.Empty; // "2-Week" or "4-Week"
    }
}