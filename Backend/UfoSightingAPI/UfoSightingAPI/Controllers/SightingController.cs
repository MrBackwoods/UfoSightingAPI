using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UfoSightingAPI.Models;

namespace UfoSightingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SightingController : ControllerBase
    {
        private readonly UfoSightingDBContext _dBContext;

        public SightingController(UfoSightingDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        // PUBLIC: Get all sightings per year with limited details
        [HttpGet("byYear/{year}")]
        public async Task<ActionResult<IEnumerable<Sighting>>> GetSightingsLimited(int year)
        {
            return await _dBContext.Sighting.Select(s => new Sighting
            {
                SightingId = s.SightingId,
                Occurred = s.Occurred,
                Latitude = s.Latitude,
                Longitude = s.Longitude

            }).Where(s => s.Occurred.Year == year).ToListAsync();
        }

        // PUBLIC: get a single sighting with limited details
        [HttpGet("byId/{id}")]
        public async Task<ActionResult<Sighting>> GetSightingLimited(int id)
        {
            var sighting = await _dBContext.Sighting.FindAsync(id);

            if (sighting == null)
            {
                return NotFound("Sighting not found.");
            }

            return new Sighting
            {
                SightingId = sighting.SightingId,
                Occurred = sighting.Occurred,
                Latitude = sighting.Latitude,
                Longitude = sighting.Longitude
            };
        }

        // PRIVATE: Get all sightings per year with full details
        [HttpGet("withDetails/byYear/{year}"), ValidatePermissions(NeedsAdminRights = false)]
        public async Task<ActionResult<IEnumerable<Sighting>>> GetSightingsWithDetails(int year)
        {
            return await _dBContext.Sighting.Where(s => s.Occurred.Year == year).ToListAsync();
        }

        // PRIVATE: Get a single sighting - with full details
        [HttpGet("withDetails/byId/{id}"), ValidatePermissions(NeedsAdminRights = false)]
        public async Task<ActionResult<Sighting>> GetSightingWithDetails(int id)
        {
            var sighting = await _dBContext.Sighting.FindAsync(id);

            if (sighting == null)
            {
                return NotFound("Sighting not found.");
            }

            return sighting;
        }

        // PRIVATE: Post a new sighting
        [HttpPost, ValidatePermissions(NeedsAdminRights = false)]
        public async Task<ActionResult<Sighting>> PostSighting(Sighting sighting)
        {
            int memberID = HttpContext.Items[ValidatePermissions._memberIDKey] as int? ?? 0;
            
            if (memberID == 0)
            {
                return BadRequest("Invalid member ID.");
            }

            sighting.Reported = DateTime.Now;
            sighting.ReportedBy = memberID;
            _dBContext.Sighting.Add(sighting);
            await _dBContext.SaveChangesAsync();
            return Ok("Sighting added successfully.");
        }

        // PRIVATE, ADMIN: Delete a sighting
        [HttpDelete("{id}"), ValidatePermissions(NeedsAdminRights = true)]
        public async Task<IActionResult> DeleteSighting(int id)
        {
            var sighting = await _dBContext.Sighting.FindAsync(id);

            if (sighting == null)
            {
                return NotFound("Sighting not found.");
            }

            _dBContext.Sighting.Remove(sighting);
            await _dBContext.SaveChangesAsync();
            return Ok("Sighting deleted successfully.");
        }

        // PRIVATE, ADMIN: Update a sighting - note that no partial updates are allowed
        [HttpPut("{id}"), ValidatePermissions(NeedsAdminRights = true)]
        public async Task<IActionResult> PutSighting(int id, Sighting sighting)
        {
            if (id != sighting.SightingId)
            {
                return BadRequest("SightingID cannot be changed.");
            }

            var orgSighting = await _dBContext.Sighting.FindAsync(id);

            if (orgSighting == null)
            {
                return NotFound("Sighting not found.");
            }

            try
            {
                _dBContext.Entry(orgSighting).CurrentValues.SetValues(sighting);
                await _dBContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Error handling update request.");
            }

            return Ok("Sighting updated successfully.");
        }
    }
}
