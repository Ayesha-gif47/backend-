using System.Collections.Generic;
using System.Threading.Tasks;
using hope4life.Data;
using hope4life.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public StatusController(ApplicationDbContext db) => _db = db;

        // GET: api/Status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusMaster>>> GetAll() =>
            Ok(await _db.StatusMasters.ToListAsync());

        // GET: api/Status/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var status = await _db.StatusMasters.FindAsync(id);
            return status == null ? NotFound() : Ok(status);
        }

        // POST: api/Status
        [HttpPost]
        public async Task<IActionResult> Create(StatusMaster model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.StatusMasters.Add(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.StatusId }, model);
        }

        // PUT: api/Status/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StatusMaster model)
        {
            if (id != model.StatusId) return BadRequest("ID mismatch");

            var existing = await _db.StatusMasters.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Status = model.Status;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/Status/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _db.StatusMasters.FindAsync(id);
            if (status == null) return NotFound();

            _db.StatusMasters.Remove(status);
            await _db.SaveChangesAsync();
            return Ok("Deleted");
        }
    }
}

