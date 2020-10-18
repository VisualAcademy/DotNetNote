using System.Collections.Generic;
using System.Linq;

namespace DotNetNote.Models
{
    public class UrlRepository : IUrlRepository
    {
        private readonly DotNetNoteContext _context;

        public UrlRepository()
        {
            _context = new DotNetNoteContext();
        }

        public UrlRepository(DotNetNoteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 이메일이 URL 중 하나라도 포함되어 있으면 true 반환
        /// </summary>
        /// <param name="email">이메일</param>
        /// <returns>SiteUrl에 포함된 이메일이면 참을 반환</returns>
        public bool IsExists(string email)
        {
            bool r = false;

            List<Url> urls = _context.Urls.ToList();
            foreach (var url in urls)
            {
                if (email.Contains(url.SiteUrl))
                {
                    return true;
                }
            }

            return r;
        }
    }
}
