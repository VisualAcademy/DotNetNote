using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DotNetSale.Models
{
    public class CategoryRepositoryAdo : ICategoryRepository
    {
        public void AddCategory(string categoryName)
        {
            throw new NotImplementedException();
        }

        public List<Category> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Category> GetCategories()
        {
            throw new NotImplementedException();
        }

        public SqlDataReader GetProductCategories()
        {
            throw new NotImplementedException();
        }
    }
}
