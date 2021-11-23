using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models.Ideas
{
    /// <summary>
    /// [2] 모델 클래스
    /// Idea 모델 클래스: Ideas 테이블과 일대일로 매핑
    /// </summary>
    public class Idea
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 노트
        /// </summary>
        [Required]
        public string Note { get; set; }
    }
}
