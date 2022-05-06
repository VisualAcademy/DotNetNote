#nullable enable
using DotNetNote.Controllers.Articles;

namespace DotNetNote.Controllers.Articles
{
    using MemoEngineCore.Models;

    using System.Collections.Generic;

    public interface IBlogService
    {
        IEnumerable<Post> GetPosts();
    }
}
