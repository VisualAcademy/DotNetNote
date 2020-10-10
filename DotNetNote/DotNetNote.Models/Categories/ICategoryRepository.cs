using System.Collections.Generic;

namespace DotNetNote.Models
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
    }
}
