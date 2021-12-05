using System.Collections.Generic;

namespace DotNetNote.Models.Categories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
    }
}
