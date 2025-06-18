/*using System;
using System.Linq;
using System.Threading.Tasks;
using hope4life.Data;
using hope4life.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CycleController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CycleController(ApplicationDbContext db) => _db = db;

        // ───────────────────────────────────────────────
        //  Called right after PATIENT registration
        // ───────────────────────────────────────────────
        [HttpPost("start/{patientId}")]
        public async Task<IActionResult> StartCycle(Guid patientId)
            => await AssignDonors(patientId);

        // ───────────────────────────────────────────────
        //  Called whenever new donors become eligible
        // ───────────────────────────────────────────────
        [HttpPost("refresh/{patientId}")]
        public async Task<IActionResult> RefreshCycle(Guid patientId)
            => await AssignDonors(patientId);

        // core logic reused by both endpoints
        private async Task<IActionResult> AssignDonors(Guid patientId)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient == null)
                return NotFound("Patient not found");

            int fullNeeded = patient.Frequency?.Equals("Monthly",
                               StringComparison.OrdinalIgnoreCase) == true ? 4 : 8;
            int minToStart = fullNeeded / 2;   // 2 or 4

            int alreadyAssigned = await _db.DonorAssignments
                                           .CountAsync(a => a.PatientId == patientId &&
                                                            a.DonorType == "Cycle");

            if (alreadyAssigned >= fullNeeded)
                return Ok(new { message = "Cycle already complete" });

            int stillNeeded = fullNeeded - alreadyAssigned;
            DateTime threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            var newDonors = await _db.Donors
                .Where(d => d.BloodGroup == patient.BloodType
                         && d.Address == patient.Address     // Address holds city
                         && d.IsAvailable
                         && (d.LastDonationDate == null || d.LastDonationDate <= threeMonthsAgo))
                .OrderBy(d => d.LastDonationDate)
                .Take(stillNeeded)
                .ToListAsync();

            if (alreadyAssigned == 0 && newDonors.Count < minToStart)
            {
                // ─────────────────────────────────────────────────────
                // DEBUG ONLY: shows which donors exist but failed match
                var debugDonors = await _db.Donors.Select(d => new
                {
                   d.Id,
                    d.BloodGroup,
                    d.Address,
                    d.IsAvailable,
                    d.LastDonationDate
                }).ToListAsync();

                return BadRequest(new
                {
                    message = $"Only {newDonors.Count} eligible donors found (need {minToStart}).",
                    availableDonors = debugDonors
                });
                // ─────────────────────────────────────────────────────
            }

            // ─── build assignment entities
            var assignments = newDonors.Select(d => new DonorAssignment
            {
                DonorAssignmentId = Guid.NewGuid(),
                PatientId = patient.Id,
                DonorId = d.Id,
                ScheduleDate = DateTime.UtcNow,
                DonorType = "Cycle",
                StatusId = 1,
                CreatedOn = DateTime.UtcNow
            }).ToList();

            // save to DB and mark donors unavailable
            _db.DonorAssignments.AddRange(assignments);
            newDonors.ForEach(d => d.IsAvailable = false);
            await _db.SaveChangesAsync();

            // prepare lightweight DTOs
            var assignmentDtos = assignments.Select(a => new
            {
                a.DonorAssignmentId,
                a.DonorId,
                a.ScheduleDate,
                a.DonorType,
                a.StatusId
            }).ToList();

            return Ok(new
            {
                donorsJustAssigned = newDonors.Count,
                totalAssigned = alreadyAssigned + newDonors.Count,
                stillNeeded = fullNeeded - (alreadyAssigned + newDonors.Count),
                assignments = assignmentDtos
            });
        }


    }
}*/
/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hope4life.Data;
using hope4life.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorAssignmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DonorAssignmentController(ApplicationDbContext ctx) => _context = ctx;

        // ─────────────────────────────────────────────────────────────
        // 1) POST: Assign new donors to patient
        // ─────────────────────────────────────────────────────────────
        [HttpPost("assign-donors/{patientId}")]
        public async Task<IActionResult> AssignDonorsToPatient(Guid patientId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient == null) return NotFound("Patient not found.");

            bool isBiMonthly = patient.Frequency == "2";  // adjust as needed
            int maxDonors = isBiMonthly ? 8 : 4;
            int minRequired = isBiMonthly ? 4 : 2;

            int assignedCount = await _context.DonorAssignments
                .CountAsync(a => a.PatientId == patientId && a.DonorType == "Cycle");

            if (assignedCount >= maxDonors)
                return Ok("Required number of donors already assigned.");

            int needed = maxDonors - assignedCount;

            var assignedIds = await _context.DonorAssignments
                .Where(a => a.PatientId == patientId && a.DonorType == "Cycle")
                .Select(a => a.DonorId)
                .ToListAsync();

            var donors = await _context.Donors
                .Where(d => d.BloodGroup == patient.BloodType
                         && d.Address == patient.Address
                         && d.IsAvailable
                         && !assignedIds.Contains(d.Id))
                .Take(needed)
                .ToListAsync();

            if (donors.Count == 0)
                return Ok("No matching donors found (check blood group / location).");

            foreach (var d in donors)
            {
                _context.DonorAssignments.Add(new DonorAssignment
                {
                    DonorAssignmentId = Guid.NewGuid(),
                    DonorId = d.Id,
                    PatientId = patient.Id,
                    ScheduleDate = DateTime.UtcNow,
                    DonorType = "Cycle",
                    StatusId = 1,
                    CreatedOn = DateTime.UtcNow
                });
                d.IsAvailable = false;
            }

            await _context.SaveChangesAsync();

            return Ok($"{donors.Count} donor(s) assigned. Still need {maxDonors - (assignedCount + donors.Count)} more.");
        }

        // ─────────────────────────────────────────────────────────────
        // 2) GET: Get all donor assignments
        // ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAllAssignments()
        {
            var all = await _context.DonorAssignments
                .Include(a => a.Donor)
                .Include(a => a.Patient)
                .ToListAsync();

            return Ok(all);
        }

        // ─────────────────────────────────────────────────────────────
        // 3) PUT: Update donor assignment status
        // ─────────────────────────────────────────────────────────────
        [HttpPut("{assignmentId}")]
        public async Task<IActionResult> UpdateAssignmentStatus(Guid assignmentId, [FromBody] int newStatusId)
        {
            var assignment = await _context.DonorAssignments.FindAsync(assignmentId);
            if (assignment == null)
                return NotFound("Assignment not found.");

            assignment.StatusId = newStatusId;
            await _context.SaveChangesAsync();

            return Ok("Assignment updated.");
        }

        // ─────────────────────────────────────────────────────────────
        // 4) DELETE: Delete donor assignment
        // ─────────────────────────────────────────────────────────────
        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(Guid assignmentId)
        {
            var assignment = await _context.DonorAssignments.FindAsync(assignmentId);
            if (assignment == null)
                return NotFound("Assignment not found.");

            _context.DonorAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return Ok("Assignment deleted.");
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
    public class DonorAssignmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonorAssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("assign/{patientId}")]
        public async Task<IActionResult> AutoAssign(Guid patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return NotFound("Patient not found.");

            var donors = await _context.Donors
                .Where(d => d.BloodGroup == patient.BloodType && d.Address == patient.Address && d.IsAvailable)
                .Take(patient.Frequency == 2 ? 8 : 4)
                .ToListAsync();

            if (!donors.Any()) return BadRequest("No matching donors found.");

            int minRequired = patient.Frequency == 2 ? 4 : 2;

            var assigned = new List<DonorAssignment>();
            for (int i = 0; i < donors.Count; i++)
            {
                assigned.Add(new DonorAssignment
                {
                    PatientId = patientId,
                    DonorId = donors[i].Id,
                    ScheduleDate = DateTime.UtcNow.AddDays(1),
                    DonorType = patient.Frequency == 2 ? "2-Week" : "4-Week",
                    StatusId = 1,
                    Remarks = "Auto-assigned"
                });
            }

            await _context.DonorAssignments.AddRangeAsync(assigned);
            await _context.SaveChangesAsync();

            if (assigned.Count >= minRequired)
                return Ok($"{assigned.Count} donors assigned. Cycle started. Remaining will be added when available.");
            else
                return Ok($"{assigned.Count} donors assigned. Waiting for more donors to start the cycle.");
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAssignments(Guid patientId)
        {
            var list = await _context.DonorAssignments
                .Where(d => d.PatientId == patientId)
                .ToListAsync();
            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _context.DonorAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _context.DonorAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return Ok("Deleted successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DonorAssignment update)
        {
            if (id != update.Id) return BadRequest("ID mismatch.");

            var exists = await _context.DonorAssignments.AsNoTracking().AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();

            update.UpdatedOn = DateTime.UtcNow;
            _context.DonorAssignments.Update(update);
            await _context.SaveChangesAsync();

            return Ok("Updated successfully.");
        }
    }
}final*/
using hope4life.Data;
using hope4life.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private const int COOLDOWN_DAYS = 90;   // 3 months ≈ 90 days

        public DonorAssignmentsController(ApplicationDbContext ctx) => _ctx = ctx;

        // ─────────────────────────────────────────────────────────────────────
        // 🧠 AUTO‑ASSIGN  →  POST api/donorassignments/auto/{patientId}
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost("auto/{patientId}")]
        public async Task<IActionResult> AutoAssign(Guid patientId)
        {
            var patient = await _ctx.Patients.FindAsync(patientId);
            if (patient is null) return NotFound("Patient not found.");

            // 1)  business constants based on frequency
            var freqWeeks = patient.Frequency;                 // 2 or 4
            var donorType = freqWeeks == 2 ? "2-Week" : "4-Week";
            var totalNeeded = freqWeeks == 2 ? 8 : 4;
            var startCycleWith = freqWeeks == 2 ? 4 : 2;
            var gapDays = freqWeeks * 7;                     // 14 or 28

            // 2)  existing assignments for this patient (current cycle)
            var existing = await _ctx.DonorAssignments
                .Where(a => a.PatientId == patientId &&
                            a.DonorType == donorType)
                .ToListAsync();

            // 2a)  if current cycle finished & cooldown over → restart
            if (existing.Any() && await CycleFinishedAndCooledDownAsync(patientId, donorType))
            {
                existing.ForEach(e => e.StatusId = 99);             // optional: mark old as 'archived'
                await _ctx.SaveChangesAsync();
                existing.Clear();
            }

            // 3)  how many more donors do we still need?
            var remaining = totalNeeded - existing.Count;
            if (remaining <= 0)
                return Ok($"✅ Cycle already full ({totalNeeded} donors).");

            // 4)  find candidate donors (blood + location match, not in active/cooldown)
            var alreadyAssignedDonorIds = await _ctx.DonorAssignments
                .Where(a => !a.Donated &&                       // still pending elsewhere
                            a.PatientId != patientId)
                .Select(a => a.DonorId)
                .ToListAsync();

            var coolDownCutoff = DateTime.UtcNow.AddDays(-COOLDOWN_DAYS);

            var cooledDownIds = await _ctx.DonorAssignments
                .Where(a => a.Donated && a.DonationDate > coolDownCutoff)
                .Select(a => a.DonorId)
                .Distinct()
                .ToListAsync();

            var candidateDonors = await _ctx.Donors
                .Where(d =>
                       d.BloodGroup == patient.BloodType &&
                       d.Address == patient.Address &&
                       !alreadyAssignedDonorIds.Contains(d.Id) &&
                       !cooledDownIds.Contains(d.Id))
                .ToListAsync();

            if (!candidateDonors.Any())
                return BadRequest("No matching donors currently available.");

            var selected = candidateDonors.Take(remaining).ToList();

            // 5)  schedule dates – continue after the latest existing one
            var lastDate = existing.Any()
                            ? existing.Max(a => a.ScheduleDate)
                            : DateTime.UtcNow;

            foreach (var donor in selected)
            {
                lastDate = lastDate.AddDays(gapDays);  // 14 or 28 days

                _ctx.DonorAssignments.Add(new DonorAssignment
                {
                    PatientId = patient.Id,
                    DonorId = donor.Id,
                    ScheduleDate = lastDate,
                    DonorType = donorType,
                    Remarks = "Auto‑assigned",
                });
            }

            await _ctx.SaveChangesAsync();

            int nowTotal = existing.Count + selected.Count;
            string msg = nowTotal >= startCycleWith
                         ? $"✅ {nowTotal} donors assigned – cycle STARTED.\n" +
                           $"Need {(totalNeeded - nowTotal)} more; they’ll be added automatically."
                         : $"⚠️  {nowTotal} donors assigned. " +
                           $"{startCycleWith - nowTotal} more to start cycle.";

            return Ok(msg);
        }

        // ─────────────────────────────────────────────────────────────────────
        // ✔ helper: has every donor donated & cooled down?
        // ─────────────────────────────────────────────────────────────────────
        private async Task<bool> CycleFinishedAndCooledDownAsync(Guid patientId, string donorType)
        {
            var assignments = await _ctx.DonorAssignments
                .Where(a => a.PatientId == patientId && a.DonorType == donorType)
                .ToListAsync();

            if (assignments.Any(a => !a.Donated)) return false;                 // still pending

            var cutoff = DateTime.UtcNow.AddDays(-COOLDOWN_DAYS);
            return assignments.All(a => a.DonationDate <= cutoff);              // everyone cooled
        }

        // ─────────────────────────────────────────────────────────────────────
        // PATCH api/donorassignments/{id}/mark-donated         (mark donation)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPatch("{id:int}/mark-donated")]
        public async Task<IActionResult> MarkDonated(int id)
        {
            var assignment = await _ctx.DonorAssignments.FindAsync(id);
            if (assignment is null) return NotFound();

            if (assignment.Donated)
                return BadRequest("Already marked donated.");

            assignment.Donated = true;
            assignment.DonationDate = DateTime.UtcNow;
            assignment.UpdatedOn = DateTime.UtcNow;

            await _ctx.SaveChangesAsync();
            return Ok("Donation recorded. Donor now in 3‑month cooldown.");
        }

        // ─────────────────────────────────────────────────────────────────────
        // CRUD ENDPOINTS
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _ctx.DonorAssignments.Include(d => d.Donor)
                                             .Include(p => p.Patient)
                                             .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var a = await _ctx.DonorAssignments.Include(d => d.Donor)
                                               .Include(p => p.Patient)
                                               .FirstOrDefaultAsync(x => x.Id == id);
            return a is null ? NotFound() : Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DonorAssignment dto)
        {
            _ctx.DonorAssignments.Add(dto);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DonorAssignment dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch.");

            var existing = await _ctx.DonorAssignments.FindAsync(id);
            if (existing is null) return NotFound();

            dto.CreatedOn = existing.CreatedOn;   // don’t lose original
            dto.UpdatedOn = DateTime.UtcNow;

            _ctx.Entry(dto).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _ctx.DonorAssignments.FindAsync(id);
            if (a is null) return NotFound();

            _ctx.DonorAssignments.Remove(a);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
