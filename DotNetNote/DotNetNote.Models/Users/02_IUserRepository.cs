//[User][3] 리포지토리 인터페이스
namespace DotNetNote.Models
{
    public interface IUserRepository
    {
        void AddUser(string userId, string password);
        UserViewModel GetUserByUserId(string userId);
        bool IsCorrectUser(string userId, string password);
        void ModifyUser(int uid, string userId, string password);
    }
}
