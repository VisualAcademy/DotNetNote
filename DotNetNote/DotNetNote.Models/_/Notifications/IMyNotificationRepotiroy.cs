namespace DotNetNote.Models.Notifications
{
    public interface IMyNotificationRepository
    {
        void CompleteNotificationByUserid(int userId);
        void CompleteNotificationByUserid(int userId, int id);
        MyNotification GetNotificationByUserid(int userId);
        bool IsNotification(int userId);
    }
}