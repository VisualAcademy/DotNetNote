using System.Collections.Generic;

namespace DotNetNote.Models
{
    /// <summary>
    /// [3] 리포지토리 인터페이스 
    /// </summary>
    public interface IIdeaRepository
    {
        /// <summary>
        /// 출력
        /// </summary>
        List<Idea> GetAll();

        /// <summary>
        /// 입력
        /// </summary>
        Idea Add(Idea model);
    }
}
