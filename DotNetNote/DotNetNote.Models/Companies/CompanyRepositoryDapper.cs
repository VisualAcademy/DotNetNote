using Dapper;
using Dul.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNetNote.Models
{
    public class CompanyRepositoryDapper : ICompanyRepository
    {
        private string _connectionString;
        private IDbConnection db;

        public CompanyRepositoryDapper(string connectionString)
        {
            _connectionString = connectionString;

            db = new SqlConnection(_connectionString);
        }

        public CompanyModel Add(CompanyModel model)
        {
            var sql =
                "Insert Into Companies (Name) Values (@Name); " +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            var id = db.Query<int>(sql, model).Single();

            model.Id = id;
            return model;
        }

        public CompanyModel Browse(int id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool Edit(CompanyModel model)
        {
            throw new NotImplementedException();
        }

        public int Has()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CompanyModel> Ordering(OrderOption orderOption)
        {
            throw new NotImplementedException();
        }

        public List<CompanyModel> Paging(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public List<CompanyModel> Read()
        {
            string sql = "Select * From Companies";
            return db.Query<CompanyModel>(sql).ToList();
        }

        public List<CompanyModel> Search(string query)
        {
            throw new NotImplementedException();
        }
    }
}
