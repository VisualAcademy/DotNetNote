using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace DotNetNote.Models.Notifications
{
    public class MyNotificationRepository : IMyNotificationRepository
    {
        private IDbConnection db;

        public MyNotificationRepository(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 특정 사용자에 대한 알림이 있는지 확인
        /// </summary>
        public bool IsNotification(int userId)
        {
            bool r = false;

            string sql = "Select Count(*) From MyNotifications Where UserId = @UserId And IsComplete = 0";
            int count = db.Query<int>(sql, new { UserId = userId }).Single();

            if (count > 0)
            {
                r = true;
            }

            return r;
        }

        /// <summary>
        /// 특정 사용자의 최신 알림 메시지 반환
        /// </summary>
        public MyNotification GetNotificationByUserid(int userId)
        {
            string sql = "Select Top 1 * From MyNotifications Where UserId = @UserId And IsComplete = 0";
            return db.Query<MyNotification>(sql, new { UserId = userId }).SingleOrDefault();
        }

        /// <summary>
        /// 특정 사용자의 특정 알림에 대해서 확인했다고 설정
        /// </summary>
        public void CompleteNotificationByUserid(int userId, int id)
        {
            string sql = "Update MyNotifications Set IsComplete = 1 Where UserId = @UserId And Id = @Id";
            db.Execute(sql, new { UserId = userId, Id = id });
        }

        /// <summary>
        /// 특정 사용자의 모든 알림을 확인으로 처리 
        /// </summary>
        public void CompleteNotificationByUserid(int userId)
        {
            string sql = "Update MyNotifications Set IsComplete = 1 Where UserId = @UserId";
            db.Execute(sql, new { UserId = userId });
        }
    }
}
