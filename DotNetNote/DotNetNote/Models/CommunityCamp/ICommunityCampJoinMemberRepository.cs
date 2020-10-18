using System.Collections.Generic;

namespace DotNetNote.Models
{
    public interface ICommunityCampJoinMemberRepository
    {
        List<CommunityCampJoinMember> GetAll();
        void AddMember(CommunityCampJoinMember model);
        void DeleteMember(CommunityCampJoinMember model);
    }
}
