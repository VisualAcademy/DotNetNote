namespace DotNetNote.Models;

public class AttendeeRepository : IAttendeeRepository
{
    private readonly IConfiguration _config;
    private SqlConnection con;

    /// <summary>
    /// 생성자의 매개 변수 주입 방식으로 Configuration 개체 주입
    /// </summary>
    public AttendeeRepository(IConfiguration config)
    {
        _config = config;
        con = new SqlConnection(
            _config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
    }

    /// <summary>
    /// 생성자의 매개 변수로 데이터베이스 연결 문자열 직접 받기 
    /// </summary>
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
