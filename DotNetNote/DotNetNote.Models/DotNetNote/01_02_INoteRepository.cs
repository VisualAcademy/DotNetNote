using System.Collections.Generic;

namespace DotNetNote.Models
{
    /// <summary>
    /// [1][2] 리포지토리 인터페이스 
    /// </summary>
    public interface INoteRepository
    {
        // 게시판에서 사용
        void Add(Note model);
        int DeleteNote(int id, string password);
        List<Note> GetAll(int page);
        int GetCountAll();
        int GetCountBySearch(string searchField, string searchQuery);
        string GetFileNameById(int id);
        Note GetNoteById(int id);
        List<Note> GetSeachAll(
            int page, string searchField, string searchQuery);
        void ReplyNote(Note model);
        void UpdateDownCount(string fileName);
        void UpdateDownCountById(int id);
        int UpdateNote(Note model);
        
        // 메인 페이지에서 사용
        List<Note> GetRecentPosts();
        List<Note> GetRecentPostsCache();
        List<Note> GetNewPhotos();
        List<Note> GetNewPhotosCache();
        List<Note> GetNoteSummaryByCategory(string category);
        List<Note> GetNoteSummaryByCategoryCache(string category);
        List<Note> GetNoteSummaryByCategoryBlog(string category);

        // 관리자 기능(페이지)에서 사용
        void Pinned(int id);
    }
}
