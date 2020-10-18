using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNetNote.Models
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
            var sql =
                "Insert Into Ideas (Note) Values (@Note); " +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            var id = db.Query<int>(sql, model).Single();

            model.Id = id;
            return model;
        }
    }
}
