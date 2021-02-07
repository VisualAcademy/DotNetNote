#nullable enable
namespace MemoEngineCore.Services
{
    using MemoEngineCore.Models;

    using System.Collections.Generic;

    public interface IBlogService
    {
        IEnumerable<Post> GetPosts();
    }
}
