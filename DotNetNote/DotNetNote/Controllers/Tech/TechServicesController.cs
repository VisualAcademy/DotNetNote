using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TechServicesController(TechContext context) : ControllerBase
{
    // GET: api/TechServices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tech>>> GetTeches() => await context.Teches.ToListAsync();

    // GET: api/TechServices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Tech>> GetTech(int id)
    {
        var tech = await context.Teches.FindAsync(id);

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

        context.Entry(tech).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
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
        context.Teches.Add(tech);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetTech", new { id = tech.Id }, tech);
    }

    // DELETE: api/TechServices/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Tech>> DeleteTech(int id)
    {
        var tech = await context.Teches.FindAsync(id);
        if (tech == null)
        {
            return NotFound();
        }

        context.Teches.Remove(tech);
        await context.SaveChangesAsync();

        return tech;
    }

    private bool TechExists(int id) => context.Teches.Any(e => e.Id == id);
}
