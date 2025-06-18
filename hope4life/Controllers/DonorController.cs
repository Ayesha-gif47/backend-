/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using hope4life.Data;
using hope4life.Models;

namespace hope4life.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public DonorController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllDonor()
        {
            var AllDonor=dbContext.Donors.ToList();
            return Ok(AllDonor);
        }
    }
}*/
/*using Microsoft.AspNetCore.Mvc;
using hope4life.Models;
using hope4life.Models.Entities;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorController : ControllerBase
    {
        // Temporary in-memory list (database baad mein connect karenge)
        private static List<Donor> donors = new List<Donor>
        {
            new Donor { FullName = "Ali Khan", BloodGroup = "B+", City = "Lahore", PhoneNumber = "03001234567", Email = "ali@example.com", LastDonationDate = DateTime.Now.AddDays(-30), IsAvailable = true },
            new Donor { FullName = "Sara Ahmed", BloodGroup = "O+", City = "Karachi", PhoneNumber = "03111234567", Email = "sara@example.com", LastDonationDate = DateTime.Now.AddDays(-15), IsAvailable = false }
        };

        [HttpGet]
        public ActionResult<List<Donor>> GetAllDonors()
        {
            return Ok(donors);
        }

        [HttpGet("{FullName}")]
        public ActionResult<Donor> GetDonorByFullName(string FullName )
        {
            var donor = donors.FirstOrDefault(d => d.FullName == FullName);
            if (donor == null)
                return NotFound("Donor not found");
            return Ok(donor);
        }

        [HttpPost]
        public ActionResult AddDonor(Donor newDonor)
        {
            newDonor.FullName  = donors + "Hello";
            donors.Add(newDonor);
            return Ok(newDonor);
        }
    }
}*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hope4life.Data;
using hope4life.Models.Entities;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDonors()
        {
            var donors = await _context.Donors.ToListAsync();
            return Ok(donors);
        }
        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonorById(Guid id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
                return NotFound("Donor not found");
            return Ok(donor);
        }
        [HttpPost]
        public async Task<IActionResult> AddDonor(Donor newDonor)
        {
            newDonor.Id = Guid.NewGuid();
            _context.Donors.Add(newDonor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDonorById), new { id = newDonor.Id }, newDonor);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonor(Guid id, Donor updatedDonor)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
                return NotFound("Donor not found");

            donor.FullName = updatedDonor.FullName;
            donor.Address = updatedDonor.Address;
            donor.PhoneNumber = updatedDonor.PhoneNumber;
            donor.Email = updatedDonor.Email;
            donor.Password = updatedDonor.Password;
            donor.BloodGroup = updatedDonor.BloodGroup;
            donor.NextDonationDate = updatedDonor.NextDonationDate;
            donor.LastDonationDate = updatedDonor.LastDonationDate;
            donor.IsBackupDonor = updatedDonor.IsBackupDonor;
            donor.IsAvailable = updatedDonor.IsAvailable;
            donor.NotAvailableCount = updatedDonor.NotAvailableCount;
            donor.IsBlocked = updatedDonor.IsBlocked;
            donor.Remarks = updatedDonor.Remarks;
            donor.UpdatedOn = DateTime.UtcNow; // Set update time

            await _context.SaveChangesAsync();
            return Ok(donor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(Guid id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
                return NotFound("Donor not found");

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return Ok("Donor deleted successfully");
        }

    }

}
