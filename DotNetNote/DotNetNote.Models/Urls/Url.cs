using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models
{
    /// <summary>
    /// Url 모델 클래스: Urls 테이블과 일대일
    /// </summary>
    public class Url
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 도메인
        /// </summary>
        [Required]
        public string SiteUrl { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 등록자
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 등록일: Created, DateReleased, CreatedAt, ...
        /// </summary>
        public DateTime Created { get; set; }
    }
}
