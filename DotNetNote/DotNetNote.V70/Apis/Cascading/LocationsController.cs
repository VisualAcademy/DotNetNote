#nullable disable
using DotNetNote.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VisualAcademy.Models;

namespace VisualAcademy.Apis;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LocationsController(ApplicationDbContext context) => _context = context;

    // GET: api/Locations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocations() => await _context.Locations.ToListAsync();

    [HttpGet("Properties/{parentId}")]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocationsByProperty(int parentId) =>
        // 특정 Property에 해당하는 Locations 정보만 읽어오기 
        await _context.Locations.Where(l => l.PropertyId == parentId).ToListAsync();

    // GET: api/Locations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Location>> GetLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);

        if (location == null)
        {
            return NotFound();
        }

        return location;
    }

    // PUT: api/Locations/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutLocation(int id, Location location)
    {
        if (id != location.Id)
        {
            return BadRequest();
        }

        _context.Entry(location).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocationExists(id))
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

    // POST: api/Locations
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Location>> PostLocation(Location location)
    {
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetLocation", new { id = location.Id }, location);
    }

    // DELETE: api/Locations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LocationExists(int id) => _context.Locations.Any(e => e.Id == id);
}
