using System.Collections.Generic;

namespace DotNetNote.Models.Categories;

public class CategoryRepositoryInMemory : ICategoryRepository
{
    public List<Category> GetCategories()
    {
        // 컬렉션 이니셜라이저를 사용하여 카테고리 리스트 만들기
        var categories = new List<Category>() 
        {
            new Category() { CategoryId = 1, CategoryName = "좋은 책" },
            new Category() { CategoryId = 2, CategoryName = "좋은 강의" },
            new Category() { CategoryId = 3, CategoryName = "좋은 컴퓨터" }
        };

        return categories;
    }
}
