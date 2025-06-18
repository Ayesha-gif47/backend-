using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hope4life.Data;
using hope4life.Models.Entities;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmergencyRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmergencyRequest
        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _context.EmergencyRequests
                .Include(r => r.Patient) // optional
                .ToListAsync();
            return Ok(requests);
        }

        // GET: api/EmergencyRequest/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            var request = await _context.EmergencyRequests
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound("Emergency request not found.");

            return Ok(request);
        }

        // POST: api/EmergencyRequest
        [HttpPost]
        public async Task<IActionResult> CreateRequest(EmergencyRequest newRequest)
        {
            newRequest.Id = Guid.NewGuid();
            newRequest.CreatedOn = DateTime.UtcNow;

            _context.EmergencyRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequestById), new { id = newRequest.Id }, newRequest);
        }

        // PUT: api/EmergencyRequest/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(Guid id, EmergencyRequest updatedRequest)
        {
            var request = await _context.EmergencyRequests.FindAsync(id);
            if (request == null)
                return NotFound("Emergency request not found.");

            request.PatientId = updatedRequest.PatientId;
            request.BloodType = updatedRequest.BloodType;
            request.RequiredOn = updatedRequest.RequiredOn;
            request.Location = updatedRequest.Location;
            request.StatusId = updatedRequest.StatusId;
            request.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // DELETE: api/EmergencyRequest/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(Guid id)
        {
            var request = await _context.EmergencyRequests.FindAsync(id);
            if (request == null)
                return NotFound("Emergency request not found.");

            _context.EmergencyRequests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok("Emergency request deleted successfully.");
        }
    }
}
