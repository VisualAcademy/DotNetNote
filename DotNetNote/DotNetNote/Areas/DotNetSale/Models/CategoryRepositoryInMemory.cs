using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DotNetSale.Models
{
    public class CategoryRepositoryInMemory : ICategoryRepository
    {
        public void AddCategory(string categoryName)
        {
            throw new System.NotImplementedException();
        }

        public List<Category> GetAll()
        {
            var categories = new List<Category>(){
                new Category { CategoryId = 1, CategoryName = "책" },
                new Category { CategoryId = 2, CategoryName = "강의" },
                new Category { CategoryId = 3, CategoryName = "컴퓨터" },
                new Category { CategoryId = 4, CategoryName = "생활용품" }
            };

            return categories;
        }

        public List<Category> GetCategories()
        {
            throw new System.NotImplementedException();
        }

        public SqlDataReader GetProductCategories()
        {
            throw new System.NotImplementedException();
        }
    }
}
