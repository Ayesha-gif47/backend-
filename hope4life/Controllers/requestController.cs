using Microsoft.AspNetCore.Mvc;
using BloodBankBackend.Models;

namespace BloodBankBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyRequestController : ControllerBase
    {
        // 📌 In-memory list to simulate DB
        private static List<Patient> patients = new List<Patient>
        {
            new Patient { Id = 1, Name = "Ali Khan", Email = "ali@example.com", Password = "1234", BloodGroup = "A+", City = "Lahore", LastDonationDate = DateTime.Now.AddDays(-20), DonationCycle = "2-Week" },
            new Patient { Id = 2, Name = "Sana Riaz", Email = "sana@example.com", Password = "4321", BloodGroup = "O-", City = "Karachi", LastDonationDate = DateTime.Now.AddDays(-25), DonationCycle = "4-Week" }
        };

        // ✅ Get all emergency patients
        [HttpGet("patients")]
        public IActionResult GetAllPatients()
        {
            return Ok(patients);
        }

        // ✅ Get patient by ID
        [HttpGet("patient/{id}")]
        public IActionResult GetPatientById(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
                return NotFound("Patient not found");

            return Ok(patient);
        }

        // ✅ Add new patient (for emergency request)
        [HttpPost("add")]
        public IActionResult AddPatient([FromBody] Patient patient)
        {
            patient.Id = patients.Count + 1;
            patients.Add(patient);
            return Ok(patient);
        }

        // ✅ Update existing patient
        [HttpPut("update/{id}")]
        public IActionResult UpdatePatient(int id, [FromBody] Patient updated)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
                return NotFound("Patient not found");

            patient.Name = updated.Name;
            patient.Email = updated.Email;
            patient.BloodGroup = updated.BloodGroup;
            patient.City = updated.City;
            patient.Password = updated.Password;
            patient.LastDonationDate = updated.LastDonationDate;
            patient.DonationCycle = updated.DonationCycle;

            return Ok(patient);
        }

        // ✅ Delete a patient
        [HttpDelete("delete/{id}")]
        public IActionResult DeletePatient(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
                return NotFound("Patient not found");

            patients.Remove(patient);
            return Ok("Patient deleted");
        }
    }
}