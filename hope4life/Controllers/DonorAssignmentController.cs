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
