namespace AttendeeApp.Models;

public class AttendeeRepository : IAttendeeRepository
{
    private readonly string _connectionString;

    public AttendeeRepository(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        _connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
    }

    public AttendeeRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("A valid connection string is required.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public void Add(Attendee model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = @"
            Insert Into Attendees (UID, UserId, Name)
            Values (@UID, @UserId, @Name);";

        using var con = CreateConnection();
        con.Execute(sql, model);
    }

    public void Delete(Attendee model)
    {
        ArgumentNullException.ThrowIfNull(model);

        const string sql = @"
            Delete From Attendees
            Where Id = @Id;";

        using var con = CreateConnection();
        con.Execute(sql, new { model.Id });
    }

    public List<Attendee> GetAll()
    {
        const string sql = @"
            Select *
            From Attendees
            Order By Id Asc;";

        using var con = CreateConnection();
        return con.Query<Attendee>(sql).ToList();
    }
}