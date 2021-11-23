using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNetNote.Models.Ideas
{
    /// <summary>
    /// [4] 리포지토리 클래스
    /// </summary>
    public class IdeaRepository : IIdeaRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        public IdeaRepository(IConfiguration config)
        {
            _config = config;

            db = new SqlConnection(_config.GetSection("ConnectionString").Value);
        }

        public List<Idea> GetAll() =>
            db.Query<Idea>("Select * From Ideas").ToList();

        public Idea Add(Idea model)
        {
            //[!] 새로운 데이터 저장 후 일련번호(Identity) 값을 반환 받아 모델 클래스에 채움
            var sql =
                "Insert Into Ideas (Note) Values (@Note); " +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            var id = db.Query<int>(sql, model).Single();

            model.Id = id;
            return model;
        }
    }
}
