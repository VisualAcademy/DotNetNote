using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DotNetNote.Models.Buyers
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly IDbConnection db;

        public BuyerRepository(string connectionString) => db = new SqlConnection(connectionString);

        public Buyer GetBuyer(string buyerId)
        {
            string sql = "Select * From Buyers Where BuyerId = @BuyerId";
            return db.Query<Buyer>(sql, new { BuyerId = buyerId }).SingleOrDefault();
        }

        public List<Buyer> GetBuyers()
        {
            string sql = "Select * From Buyers Order By Id Desc";
            return db.Query<Buyer>(sql).ToList();
        }
    }
}
