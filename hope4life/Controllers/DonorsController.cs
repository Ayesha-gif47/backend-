using Microsoft.AspNetCore.Mvc;
using BloodBankBackend.Models;

namespace BloodBankBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorController : ControllerBase
    {
        // Temporary in-memory list (database baad mein connect karenge)
        private static List<Donor> donors = new List<Donor>
        {
            new Donor { Id = 1, FullName = "Ali Khan", BloodGroup = "B+", City = "Lahore", PhoneNumber = "03001234567", Email = "ali@example.com", LastDonationDate = DateTime.Now.AddDays(-30), IsAvailable = true },
            new Donor { Id = 2, FullName = "Sara Ahmed", BloodGroup = "O+", City = "Karachi", PhoneNumber = "03111234567", Email = "sara@example.com", LastDonationDate = DateTime.Now.AddDays(-15), IsAvailable = false }
        };

        [HttpGet]
        public ActionResult<List<Donor>> GetAllDonors()
        {
            return Ok(donors);
        }

        [HttpGet("{id}")]
        public ActionResult<Donor> GetDonorById(int id)
        {
            var donor = donors.FirstOrDefault(d => d.Id == id);
            if (donor == null)
                return NotFound("Donor not found");
            return Ok(donor);
        }

        [HttpPost]
        public ActionResult AddDonor(Donor newDonor)
        {
            newDonor.Id = donors.Count + 1;
            donors.Add(newDonor);
            return Ok(newDonor);
        }
    }
}