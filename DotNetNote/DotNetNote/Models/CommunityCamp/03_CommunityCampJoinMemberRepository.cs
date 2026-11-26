namespace DotNetNote.Models;

public class CommunityCampJoinMemberRepository :
    ICommunityCampJoinMemberRepository
{
    private readonly IConfiguration _config;
    private SqlConnection _db;

    public CommunityCampJoinMemberRepository(IConfiguration config)
    {
        _config = config;
        _db = new SqlConnection(_config.GetConnectionString("DefaultConnection") ?? throw new System.InvalidOperationException("Connection string 'DefaultConnection' is not configured."));
    }

    public List<CommunityCampJoinMember> GetAll() =>
        _db.Query<CommunityCampJoinMember>(
            "Select * From CommunityCampJoinMembers Order By Id Asc").ToList();

    public void AddMember(CommunityCampJoinMember model) =>
        _db.Execute("Insert Into CommunityCampJoinMembers "
            + " (CommunityName, Name, Mobile, Email, Size, CreationDate) "
            + " Values (@CommunityName, @Name, @Mobile, @Email, @Size, GetDate())",
            model);

    public void DeleteMember(CommunityCampJoinMember model)
    {
        _db.Execute("Delete CommunityCampJoinMembers Where "
            + " CommunityName = @CommunityName And Name = @Name And "
            + "Mobile = @Mobile And Email = @Email", model);
    }
}
