#nullable disable
using Acts.Models;

namespace Acts.Controllers.ActionCategories;

[Route("api/[controller]")]
[ApiController]
public class ActionCategoriesController(ActContext context) : ControllerBase
{
    // GET: api/ActionCategories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActionCategory>>> GetActionCategories() => await context.ActionCategories.ToListAsync();

    // GET: api/ActionCategories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ActionCategory>> GetActionCategory(long id)
    {
        var actionCategory = await context.ActionCategories.FindAsync(id);

        if (actionCategory == null)
        {
            return NotFound();
        }

        return actionCategory;
    }

    // PUT: api/ActionCategories/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutActionCategory(long id, ActionCategory actionCategory)
    {
        if (id != actionCategory.Id)
        {
            return BadRequest();
        }

        context.Entry(actionCategory).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ActionCategoryExists(id))
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

    // POST: api/ActionCategories
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ActionCategory>> PostActionCategory(ActionCategory actionCategory)
    {
        context.ActionCategories.Add(actionCategory);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetActionCategory", new { id = actionCategory.Id }, actionCategory);
    }

    // DELETE: api/ActionCategories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActionCategory(long id)
    {
        var actionCategory = await context.ActionCategories.FindAsync(id);
        if (actionCategory == null)
        {
            return NotFound();
        }

        context.ActionCategories.Remove(actionCategory);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ActionCategoryExists(long id) => context.ActionCategories.Any(e => e.Id == id);
}
