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
