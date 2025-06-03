using Microsoft.AspNetCore.Mvc;
using BloodBankBackend.Models;

namespace BloodBankBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        // Temporary in-memory list (database baad mein connect karenge)
        private static List<Patient> patients = new List<Patient>
        {
            new Patient { Id = 1, Name = "Ahmed Raza", Email = "ahmed@example.com", Password = "1234", BloodGroup = "B+", City = "Lahore", LastDonationDate = DateTime.Now.AddDays(-20), DonationCycle = "2-Week" },
            new Patient { Id = 2, Name = "Fatima Noor", Email = "fatima@example.com", Password = "5678", BloodGroup = "O+", City = "Karachi", LastDonationDate = DateTime.Now.AddDays(-25), DonationCycle = "4-Week" }
        };

        private static List<Donor> donors = new List<Donor>
        {
            new Donor { Id = 1, FullName = "Ali Khan", BloodGroup = "B+", City = "Lahore", PhoneNumber = "03001234567", Email = "ali@example.com", LastDonationDate = DateTime.Now.AddDays(-30), IsAvailable = true },
            new Donor { Id = 2, FullName = "Sara Ahmed", BloodGroup = "O+", City = "Karachi", PhoneNumber = "03111234567", Email = "sara@example.com", LastDonationDate = DateTime.Now.AddDays(-15), IsAvailable = true },
            new Donor { Id = 3, FullName = "Zain", BloodGroup = "B+", City = "Lahore", PhoneNumber = "03221234567", Email = "zain@example.com", LastDonationDate = DateTime.Now.AddDays(-10), IsAvailable = false }
        };

        [HttpPost("register")]
        public IActionResult Register(Patient patient)
        {
            patient.Id = patients.Count + 1;
            patients.Add(patient);
            return Ok(patient);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Patient login)
        {
            var patient = patients.FirstOrDefault(p => p.Email == login.Email && p.Password == login.Password);
            if (patient == null)
                return Unauthorized("Invalid email or password.");

            return Ok(patient);
        }

        [HttpGet("{id}/matched-donors")]
        public IActionResult GetMatchedDonors(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
                return NotFound("Patient not found");

            int days = patient.DonationCycle == "2-Week" ? 14 : 28;
            var nextDonationDate = patient.LastDonationDate.AddDays(days);

            var matchedDonors = donors.Where(d =>
                d.BloodGroup == patient.BloodGroup &&
                d.City == patient.City &&
                d.IsAvailable &&
                d.LastDonationDate <= nextDonationDate
            ).ToList();

            return Ok(matchedDonors);
        }
    }
}
