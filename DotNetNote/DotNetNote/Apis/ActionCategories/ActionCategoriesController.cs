#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Acts.Models;

namespace Acts.Controllers.ActionCategories
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionCategoriesController : ControllerBase
    {
        private readonly ActContext _context;

        public ActionCategoriesController(ActContext context)
        {
            _context = context;
        }

        // GET: api/ActionCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActionCategory>>> GetActionCategories()
        {
            return await _context.ActionCategories.ToListAsync();
        }

        // GET: api/ActionCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActionCategory>> GetActionCategory(long id)
        {
            var actionCategory = await _context.ActionCategories.FindAsync(id);

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

            _context.Entry(actionCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            _context.ActionCategories.Add(actionCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActionCategory", new { id = actionCategory.Id }, actionCategory);
        }

        // DELETE: api/ActionCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActionCategory(long id)
        {
            var actionCategory = await _context.ActionCategories.FindAsync(id);
            if (actionCategory == null)
            {
                return NotFound();
            }

            _context.ActionCategories.Remove(actionCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActionCategoryExists(long id)
        {
            return _context.ActionCategories.Any(e => e.Id == id);
        }
    }
}
