using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace DotNetNote.Models
{
    public class AttendeeRepository : IAttendeeRepository
    {
        private readonly IConfiguration _config;
        private SqlConnection con;

        public AttendeeRepository(IConfiguration config)
        {
            _config = config;
            con = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
        }

        public AttendeeRepository(string connectionString) => con = new SqlConnection(connectionString);

        public void Add(Attendee model)
        {
            var sql =
                "Insert Into Attendees (UID, UserId, Name) "
                + " Values (@UID, @UserId, @Name)";
            con.Execute(sql, model);
        }

        public void Delete(Attendee model)
        {
            var sql = "Delete Attendees Where Id = @Id";
            con.Execute(sql, new { Id = model.Id });
        }

        public List<Attendee> GetAll()
        {
            var sql = "Select * From Attendees Order By Id Asc";
            return con.Query<Attendee>(sql).ToList();
        }
    }
}
