using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetNote.Models;

namespace DotNetNote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechServicesController : ControllerBase
    {
        private readonly TechContext _context;

        public TechServicesController(TechContext context)
        {
            _context = context;
        }

        // GET: api/TechServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tech>>> GetTeches()
        {
            return await _context.Teches.ToListAsync();
        }

        // GET: api/TechServices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tech>> GetTech(int id)
        {
            var tech = await _context.Teches.FindAsync(id);

            if (tech == null)
            {
                return NotFound();
            }

            return tech;
        }

        // PUT: api/TechServices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTech(int id, Tech tech)
        {
            if (id != tech.Id)
            {
                return BadRequest();
            }

            _context.Entry(tech).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TechExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TechServices
        [HttpPost]
        public async Task<ActionResult<Tech>> PostTech(Tech tech)
        {
            _context.Teches.Add(tech);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTech", new { id = tech.Id }, tech);
        }

        // DELETE: api/TechServices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tech>> DeleteTech(int id)
        {
            var tech = await _context.Teches.FindAsync(id);
            if (tech == null)
            {
                return NotFound();
            }

            _context.Teches.Remove(tech);
            await _context.SaveChangesAsync();

            return tech;
        }

        private bool TechExists(int id)
        {
            return _context.Teches.Any(e => e.Id == id);
        }
    }
}
