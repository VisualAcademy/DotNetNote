using System;

namespace DotNetNote.Models
{
    /// <summary>
    /// Maxim 모델 클래스: Maxims 테이블과 일대일
    /// </summary>
    public class Maxim
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
