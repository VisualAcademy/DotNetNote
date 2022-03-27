#nullable disable
using DotNetNote.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VisualAcademy.Models;

namespace VisualAcademy.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class SublocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SublocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Sublocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sublocation>>> GetSublocations()
        {
            return await _context.Sublocations.ToListAsync();
        }

        [HttpGet("Locations/{parentId}")]
        public async Task<ActionResult<IEnumerable<Sublocation>>> GetSublocations(int parentId)
        {
            // 특정 Locations에 해당하는 Sublocations만 읽어오기 
            return await _context.Sublocations.Where(s => s.LocationId == parentId).ToListAsync();
        }

        // GET: api/Sublocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sublocation>> GetSublocation(int id)
        {
            var sublocation = await _context.Sublocations.FindAsync(id);

            if (sublocation == null)
            {
                return NotFound();
            }

            return sublocation;
        }

        // PUT: api/Sublocations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSublocation(int id, Sublocation sublocation)
        {
            if (id != sublocation.Id)
            {
                return BadRequest();
            }

            _context.Entry(sublocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SublocationExists(id))
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

        // POST: api/Sublocations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sublocation>> PostSublocation(Sublocation sublocation)
        {
            _context.Sublocations.Add(sublocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSublocation", new { id = sublocation.Id }, sublocation);
        }

        // DELETE: api/Sublocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSublocation(int id)
        {
            var sublocation = await _context.Sublocations.FindAsync(id);
            if (sublocation == null)
            {
                return NotFound();
            }

            _context.Sublocations.Remove(sublocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SublocationExists(int id)
        {
            return _context.Sublocations.Any(e => e.Id == id);
        }
    }
}
