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
public class PropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PropertiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Properties
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
    {
        return await _context.Properties.ToListAsync();
    }

    // GET: api/Properties/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Property>> GetProperty(int id)
    {
        var @property = await _context.Properties.FindAsync(id);

        if (@property == null)
        {
            return NotFound();
        }

        return @property;
    }

    // PUT: api/Properties/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProperty(int id, Property @property)
    {
        if (id != @property.Id)
        {
            return BadRequest();
        }

        _context.Entry(@property).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PropertyExists(id))
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

    // POST: api/Properties
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Property>> PostProperty(Property @property)
    {
        _context.Properties.Add(@property);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProperty", new { id = @property.Id }, @property);
    }

    // DELETE: api/Properties/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var @property = await _context.Properties.FindAsync(id);
        if (@property == null)
        {
            return NotFound();
        }

        _context.Properties.Remove(@property);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PropertyExists(int id)
    {
        return _context.Properties.Any(e => e.Id == id);
    }
}
