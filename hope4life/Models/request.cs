namespace BloodBankBackend.Models
{
    public class EmergencyRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime RequiredDate { get; set; } // The date when blood is needed
    }
}