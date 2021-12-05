using System.Collections.Generic;

namespace DotNetNote.Models.Categories
{
    public class CategoryRepositorySqlServer : ICategoryRepository
    {
        public List<Category> GetCategories()
        {
            var categories = new List<Category>()
            {
                new Category() { CategoryId = 1, CategoryName = "[DB에서] 좋은 책" },
                new Category() { CategoryId = 2, CategoryName = "[DB에서] 좋은 강의" },
                new Category() { CategoryId = 3, CategoryName = "[DB에서] 좋은 컴퓨터" }
            };

            return categories;
        }
    }
}
