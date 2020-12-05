//[User][4]
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DotNetNote.Models
{
    public class UserRepository : IUserRepository
    {
        private IConfiguration _config;
        private SqlConnection con;

        public UserRepository(string connectionString)
        {
            con = new SqlConnection(connectionString);
        }

        public UserRepository(IConfiguration config)
        {
            _config = config;
            // appsettings.json 파일에 설정된 데이터베이스 연결 문자열 읽어오기
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

        /// <summary>
        /// 회원 가입
        /// </summary>
        public void AddUser(string userId, string password)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "WriteUsers";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// 특정 회원 정보
        /// </summary>
        public UserViewModel GetUserByUserId(string userId)
        {
            UserViewModel r = new UserViewModel();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "Select * From Users Where UserID = @UserID";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserID", userId);

            con.Open();
            IDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                r.Id = dr.GetInt32(0);
                r.UserId = dr.GetString(1);
                r.Password = dr.GetString(2);
            }
            con.Close();

            return r;
        }

        /// <summary>
        /// 회원 정보 수정
        /// </summary>
        public void ModifyUser(int uid, string userId, string password)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "ModifyUsers";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@UID", uid);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// 아이디와 암호가 동일한 사용자면 참(true) 그렇지 않으면 거짓
        /// </summary>
        public bool IsCorrectUser(string userId, string password)
        {
            bool result = false;

            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "Select * From Users "
                + " Where UserID = @UserID And Password = @Password";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Password", password);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = true; // 아이디와 암호가 맞는 데이터가 있구나...
            }
            con.Close();
            return result;
        }
    }
}
