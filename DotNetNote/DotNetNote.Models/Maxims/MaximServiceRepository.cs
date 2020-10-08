using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNote.Models
{
    /// <summary>
    /// 명언(Maxim) 서비스에 대한 DB 연동 코드 부분
    /// </summary>
    public class MaximServiceRepository
    {
        // Database 개체 생성
        private SqlConnection db;
        private IConfiguration _config;

        public MaximServiceRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("Data").GetSection("DefaultConnection")
                .GetSection("ConnectionString").Value);
        }

        // 입력
        public Maxim AddMaxim(Maxim model)
        {
            string sql = @"
                Insert Into Maxims (Name, Content) Values (@Name, @Content);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
            var id = this.db.Query<int>(sql, model).Single();
            model.Id = id;
            return model;
        }

        // 출력
        public List<Maxim> GetMaxims()
        {
            string sql = @"Select Id, Name, Content, CreationDate 
                From Maxims Order By Id Asc";
            return this.db.Query<Maxim>(sql).ToList();
        }

        // 상세
        public Maxim GetMaximById(int id)
        {
            string sql = @"Select Id, name, Content, CreationDate 
                From Maxims Where Id = @Id";
            return this.db.Query<Maxim>(sql, new { Id = id }).SingleOrDefault();
        }

        // 수정
        public Maxim UpdateMaxim(Maxim model)
        {
            string sql = @"Update Maxims 
                Set Name = @Name, Content = @Content Where Id = @Id";
            this.db.Execute(sql, model);
            return model;
        }

        // 삭제
        public void RemoveMaxim(int id)
        {
            string sql = "Delete Maxims Where Id = @Id";
            this.db.Execute(sql, new { id });
        }
    }
}
