using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetNote.Models
{
    public interface ITechRepositoryEf
    {
        Task DeleteTechAsync(int id);
        Task<Tech> GetTechAsync(int id);
        Task<IEnumerable<Tech>> GetTechesAsync();
        Task PostTechAsync(Tech tech);
        Task PutTechAsync(int id, Tech tech);
    }
}