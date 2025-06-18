/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hope4life.Data;
using hope4life.Models.Entities;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _context.Patients.ToListAsync();
            return Ok(patients);
        }

        // GET: api/Patient/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound("Patient not found");

            return Ok(patient);
        }

        // POST: api/Patient
        /*[HttpPost]
        public async Task<IActionResult> AddPatient(Patient newPatient)
        {
            newPatient.Id = Guid.NewGuid();
            newPatient.CreatedOn = DateTime.UtcNow;
            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatientById), new { id = newPatient.Id }, newPatient);
        }*/
        


       /* // PUT: api/Patient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid id, Patient updatedPatient)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound("Patient not found");

            patient.Name = updatedPatient.Name;
            patient.Address = updatedPatient.Address;
            patient.PhoneNumber = updatedPatient.PhoneNumber;
            patient.GuardianName = updatedPatient.GuardianName;
            patient.Relationship = updatedPatient.Relationship;
            patient.Email = updatedPatient.Email;
            patient.Password = updatedPatient.Password;
            patient.BloodType = updatedPatient.BloodType;
            patient.Frequency = updatedPatient.Frequency;
            patient.IsActive = updatedPatient.IsActive;
            patient.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        // DELETE: api/Patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound("Patient not found");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Ok("Patient deleted successfully");
        }
    }
}*/

/*using hope4life.Data;
using hope4life.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PatientsController(ApplicationDbContext context) => _context = context;

        // GET: api/patients
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _context.Patients.ToListAsync());

        // GET: api/patients/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            return patient is null ? NotFound() : Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            if (patient.FrequencyInWeeks is not (2 or 4))
                return BadRequest("FrequencyInWeeks must be 2 or 4.");

            patient.Id = Guid.NewGuid();          // primary key set
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = patient.Id }, patient);
        }

        // PUT: api/patients/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Patient patient)
        {
            if (id != patient.Id) return BadRequest("Id mismatch.");

            if (patient.FrequencyInWeeks is not (2 or 4))
                return BadRequest("FrequencyInWeeks must be 2 or 4.");

            var existing = await _context.Patients.AsNoTracking()
                                                  .FirstOrDefaultAsync(p => p.Id == id);
            if (existing is null) return NotFound();

            patient.UpdatedOn = DateTime.UtcNow;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/patients/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null) return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}*/
using hope4life.Data;                 // ⬅️  DbContext namespace
using hope4life.Models.Entities;      // ⬅️  Patient entity namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context) => _context = context;

        // ------------------------------------------------------------------
        // GET: api/patients
        // Returns only active patients by default. Pass ?includeInactive=true
        // if you need everyone.
        // ------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var query = _context.Patients.AsQueryable();
            if (!includeInactive) query = query.Where(p => p.IsActive);

            var list = await query.ToListAsync();
            return Ok(list);
        }

        // ------------------------------------------------------------------
        // GET: api/patients/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            return patient is null ? NotFound() : Ok(patient);
        }

        // ------------------------------------------------------------------
        // POST: api/patients
        // Creates a new patient record.
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            // Business rule: we accept only 2‑ or 4‑week frequencies.
            if (patient.Frequency is not (2 or 4))
                return BadRequest("FrequencyInWeeks must be 2 or 4.");

            patient.Id = Guid.NewGuid();
            patient.CreatedOn = DateTime.UtcNow;
            patient.IsActive = true;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = patient.Id }, patient);
        }

        // ------------------------------------------------------------------
        // PUT: api/patients/{id}
        // Full update (replace) of an existing patient.
        // ------------------------------------------------------------------
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Patient patient)
        {
            if (id != patient.Id) return BadRequest("Id mismatch.");
            if (patient.Frequency is not (2 or 4))
                return BadRequest("FrequencyInWeeks must be 2 or 4.");

            var existing = await _context.Patients.AsNoTracking()
                                                  .FirstOrDefaultAsync(p => p.Id == id);
            if (existing is null) return NotFound();

            patient.CreatedOn = existing.CreatedOn;          // keep original
            patient.UpdatedOn = DateTime.UtcNow;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ------------------------------------------------------------------
        // PATCH: api/patients/{id}/deactivate
        // Soft‑delete → sets IsActive = false.
        // ------------------------------------------------------------------
        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null) return NotFound();

            if (!patient.IsActive) return BadRequest("Patient already inactive.");

            patient.IsActive = false;
            patient.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ------------------------------------------------------------------
        // DELETE: api/patients/{id}
        // Hard delete (physical removal). Use with caution.
        // ------------------------------------------------------------------
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null) return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}



