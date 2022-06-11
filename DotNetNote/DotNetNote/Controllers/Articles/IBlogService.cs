#nullable enable

namespace DotNetNote.Controllers.Articles;

using MemoEngineCore.Models;

using System.Collections.Generic;

public interface IBlogService
{
    IEnumerable<Post> GetPosts();
}
