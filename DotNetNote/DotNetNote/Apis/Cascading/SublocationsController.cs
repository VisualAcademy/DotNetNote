#nullable disable
namespace VisualAcademy.Apis;

[Route("api/[controller]")]
[ApiController]
public class SublocationsController(ApplicationDbContext context) : ControllerBase
{
    // GET: api/Sublocations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sublocation>>> GetSublocations() => await context.Sublocations.ToListAsync();

    [HttpGet("Locations/{parentId}")]
    public async Task<ActionResult<IEnumerable<Sublocation>>> GetSublocations(int parentId) =>
        // 특정 Locations에 해당하는 Sublocations만 읽어오기 
        await context.Sublocations.Where(s => s.LocationId == parentId).ToListAsync();

    // GET: api/Sublocations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Sublocation>> GetSublocation(int id)
    {
        var sublocation = await context.Sublocations.FindAsync(id);

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

        context.Entry(sublocation).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
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
        context.Sublocations.Add(sublocation);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetSublocation", new { id = sublocation.Id }, sublocation);
    }

    // DELETE: api/Sublocations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSublocation(int id)
    {
        var sublocation = await context.Sublocations.FindAsync(id);
        if (sublocation == null)
        {
            return NotFound();
        }

        context.Sublocations.Remove(sublocation);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool SublocationExists(int id) => context.Sublocations.Any(e => e.Id == id);
}
