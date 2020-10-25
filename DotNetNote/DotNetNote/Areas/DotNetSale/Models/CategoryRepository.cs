/*
    작성자: 박용준 
    타이틀: 닷넷세일 - 카테고리 리포지토리 클래스 
    코멘트: 닷넷세일 - 카테고리 리포지토리 클래스 
    수정일: 2020-11-01
*/
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace DotNetSale.Models
{
    public class CategoryRepository : ICategoryRepository
    {
        private IDbConnection db;
        public CategoryRepository()
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        }

        /// <summary>
        /// 카테고리(대분류) 등록
        /// </summary>
        /// <param name="categoryName">카테고리 이름</param>
        public void AddCategory(string categoryName)
        {
            string sql = "Insert Into Categories (CategoryName) Values (@CategoryName);";
            db.Execute(sql, new { CategoryName = categoryName });
        }

        /// <summary>
        /// 전체 카테고리 리스트 출력
        /// </summary>
        /// <returns>전체 카테고리 리스트 출력</returns>
        public List<Category> GetAll()
        {
            string sql = "Select CategoryId, CategoryName From Categories Order By CategoryId Desc";
            return db.Query<Category>(sql).ToList();
        }

        /// <summary>
        /// 전체 카테고리 리스트 출력
        /// </summary>
        /// <returns>전체 카테고리 리스트 출력</returns>
        public List<Category> GetCategories()
        {
            string sql = "Select CategoryId, CategoryName From Categories Order By CategoryId Desc";
            return db.Query<Category>(sql).ToList();
        }

        /// <summary>
        /// 전체 카테고리 리스트
        /// CategoryListUserControl.ascx에서 사용
        /// </summary>
        /// <returns>카테고리 리스트</returns>
        public SqlDataReader GetProductCategories()
        {
            #region ADO.NET 클래스 사용
            SqlConnection objCon =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            objCon.Open();

            SqlCommand objCmd = new SqlCommand("ProductCategoryList", objCon);
            objCmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader result = objCmd.ExecuteReader(CommandBehavior.CloseConnection);

            return result;
            #endregion
        }
    }
}
