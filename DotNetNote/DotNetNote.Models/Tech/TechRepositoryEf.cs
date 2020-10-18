using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetNote.Models
{
    public class TechRepositoryEf : ITechRepositoryEf
    {
        private readonly TechContext _context;

        public TechRepositoryEf()
        {
            _context = new TechContext();
        }

        public TechRepositoryEf(TechContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tech>> GetTechesAsync()
        {
            return await _context.Teches.ToListAsync();
        }

        public async Task<Tech> GetTechAsync(int id)
        {
            var tech = await _context.Teches.FindAsync(id);

            return tech;
        }

        public async Task PutTechAsync(int id, Tech tech)
        {
            var techOrg = await _context.Teches.FindAsync(id);
            if (techOrg != null)
            {
                techOrg.Title = tech.Title;
                _context.Entry(techOrg).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task PostTechAsync(Tech tech)
        {
            _context.Teches.Add(tech);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTechAsync(int id)
        {
            var tech = await _context.Teches.FindAsync(id);
            if (tech != null)
            {
                _context.Teches.Remove(tech);
                await _context.SaveChangesAsync();
            }
        }
    }
}
