#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetNote.Data;
using DotNetNote.Models;

namespace DotNetNote.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabinetTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CabinetTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CabinetTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CabinetType>>> GetCabinetTypes()
        {
            return await _context.CabinetTypes.ToListAsync();
        }

        // GET: api/CabinetTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CabinetType>> GetCabinetType(long id)
        {
            var cabinetType = await _context.CabinetTypes.FindAsync(id);

            if (cabinetType == null)
            {
                return NotFound();
            }

            return cabinetType;
        }

        // PUT: api/CabinetTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCabinetType(long id, CabinetType cabinetType)
        {
            if (id != cabinetType.Id)
            {
                return BadRequest();
            }

            _context.Entry(cabinetType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabinetTypeExists(id))
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

        // POST: api/CabinetTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CabinetType>> PostCabinetType(CabinetType cabinetType)
        {
            _context.CabinetTypes.Add(cabinetType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCabinetType", new { id = cabinetType.Id }, cabinetType);
        }

        // DELETE: api/CabinetTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinetType(long id)
        {
            var cabinetType = await _context.CabinetTypes.FindAsync(id);
            if (cabinetType == null)
            {
                return NotFound();
            }

            _context.CabinetTypes.Remove(cabinetType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CabinetTypeExists(long id)
        {
            return _context.CabinetTypes.Any(e => e.Id == id);
        }
    }
}
