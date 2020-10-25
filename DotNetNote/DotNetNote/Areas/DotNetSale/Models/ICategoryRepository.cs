/*
    작성자: 박용준 
    타이틀: 닷넷세일 - 카테고리 리포지토리 인터페이스 
    코멘트: 닷넷세일 - 카테고리 리포지토리 인터페이스 
    수정일: 2020-11-01
*/
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DotNetSale.Models
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// 카테고리 전체 정보 읽어오기
        /// </summary>
        List<Category> GetAll();
        List<Category> GetCategories();
        void AddCategory(string categoryName);

        SqlDataReader GetProductCategories();
    }
}
