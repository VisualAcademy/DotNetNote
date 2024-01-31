#nullable disable
using Microsoft.EntityFrameworkCore;
using VisualAcademy.Models;

namespace VisualAcademy.Apis;

[Route("api/[controller]")]
[ApiController]
public class LocationsController(ApplicationDbContext context) : ControllerBase
{
    // GET: api/Locations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocations() => await context.Locations.ToListAsync();

    [HttpGet("Properties/{parentId}")]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocationsByProperty(int parentId) =>
        // 특정 Property에 해당하는 Locations 정보만 읽어오기 
        await context.Locations.Where(l => l.PropertyId == parentId).ToListAsync();

    // GET: api/Locations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Location>> GetLocation(int id)
    {
        var location = await context.Locations.FindAsync(id);

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

        context.Entry(location).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
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
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetLocation", new { id = location.Id }, location);
    }

    // DELETE: api/Locations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        context.Locations.Remove(location);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool LocationExists(int id) => context.Locations.Any(e => e.Id == id);
}
