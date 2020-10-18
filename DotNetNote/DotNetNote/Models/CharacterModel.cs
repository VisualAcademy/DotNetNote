using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace DotNetNote.Models
{
    /// <summary>
    /// 모델 클래스
    /// </summary>
    public class CharacterModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int HeroId { get; set; }
    }

    /// <summary>
    /// 리포지토리 인터페이스
    /// </summary>
    public interface ICharacterRepository
    {
        CharacterModel SetCharacter(CharacterModel model);
        CharacterModel GetCharacterByUsername(string username);
    }

    /// <summary>
    /// 리포지터리 클래스
    /// </summary>
    public class CharacterRepository : ICharacterRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        /// <summary>
        /// 생성자 
        /// </summary>
        public CharacterRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(
                _config.GetSection("ConnectionStrings")
                    .GetSection("DefaultConnection").Value);
        }

        /// <summary>
        /// 캐릭터 선택(처음 입력 또는 업데이트)
        /// </summary>
        public CharacterModel SetCharacter(CharacterModel model)
        {
            string sql = ""; 
            if (GetRecordCounts(model.Username) > 0)
            {
                // 이미 저장된 캐릭터: Update
                sql =
                    "Update Characters      " +
                    "Set                    " +
                    "   HeroId = @HeroId       " +
                    "Where Username = @Username ";
                db.Execute(sql, model);
                return model;
            }
            else
            {
                // 아직 캐릭터가 저장되지 않음: Insert
                sql = @"
                Insert Into Characters (Username, HeroId) Values (@Username, @HeroId);
                Select Cast(SCOPE_IDENTITY() As Int);
                ";
                var id = db.Query<int>(sql, model).Single();
                model.Id = id;
                return model;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 특정 사용자ID에 해당하는 캐릭터 설정이 있는지 확인
        /// </summary>
        public int GetRecordCounts(string username)
        {
            string query = "Select Count(*) From Characters Where Username = @Username";
            return db.Query<int>(query, new { Username = username }).FirstOrDefault();
        }

        /// <summary>
        /// 특정 사용자의 캐릭터 정보 보기
        /// </summary>
        public CharacterModel GetCharacterByUsername(string username)
        {
            if (GetRecordCounts(username) > 0)
            {
                string query = "Select * From Characters Where Username = @Username";
                return db.Query<CharacterModel>(query, new { Username = username }).Single();
            }
            else
            {
                return null; 
            }
        }
    }
}
