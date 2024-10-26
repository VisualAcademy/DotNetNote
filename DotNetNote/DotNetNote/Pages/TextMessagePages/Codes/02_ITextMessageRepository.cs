using DotNetNote.Pages.TextMessagePages.Codes;
using Dul.Articles;
using Dul.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VisualAcademy.Models.TextMessages
{
    public interface ITextMessageCrudRepository<T> : ICrudRepositoryBase<TextMessageModel, long>
    {
        /// <summary>
        /// 모델 수정
        /// </summary>
        Task<bool> EditAsync(T model);

        /// <summary>
        /// 기본 답변 추가
        /// </summary>
        Task<T> AddAsync(
            T model,
            int parentRef,
            int parentStep,
            int parentRefOrder);

        /// <summary>
        /// 고급 답변 추가
        /// </summary>
        Task<T> AddAsync(
            T model,
            int parentId);

        /// <summary>
        /// 페이징을 통한 전체 조회
        /// </summary>
        Task<PagingResult<T>> GetAllAsync(
            int pageIndex,
            int pageSize);

        /// <summary>
        /// 부모 Id에 의한 페이징 전체 조회
        /// </summary>
        Task<PagingResult<T>> GetAllByParentIdAsync(
            int pageIndex,
            int pageSize,
            int parentId);

        /// <summary>
        /// 부모 Key에 의한 페이징 전체 조회
        /// </summary>
        Task<PagingResult<T>> GetAllByParentKeyAsync(
            int pageIndex,
            int pageSize,
            string parentKey);

        /// <summary>
        /// 페이징을 통한 검색 조회
        /// </summary>
        Task<PagingResult<T>> SearchAllAsync(
            int pageIndex,
            int pageSize,
            string searchQuery);

        /// <summary>
        /// 부모 Id에 의한 페이징 검색 조회
        /// </summary>
        Task<PagingResult<T>> SearchAllByParentIdAsync(
            int pageIndex,
            int pageSize,
            string searchQuery,
            int parentId);

        /// <summary>
        /// 부모 Key에 의한 페이징 검색 조회
        /// </summary>
        Task<PagingResult<T>> SearchAllByParentKeyAsync(
            int pageIndex,
            int pageSize,
            string searchQuery,
            string parentKey);
    }

    public interface ITextMessageRepository : ITextMessageCrudRepository<TextMessageModel>
    {
        /// <summary>
        /// 필터링 옵션에 따른 조회
        /// </summary>
        Task<ArticleSet<TextMessageModel, long>> GetByAsync<TParentIdentifier>(FilterOptions<TParentIdentifier> options);

        /// <summary>
        /// 부모 Id에 따른 상태 조회
        /// </summary>
        Task<Tuple<int, int>> GetStatus(int parentId);

        /// <summary>
        /// 부모 Id에 의한 전체 삭제
        /// </summary>
        Task<bool> DeleteAllByParentId(int parentId);

        /// <summary>
        /// 월별 생성 수 조회
        /// </summary>
        Task<SortedList<int, double>> GetMonthlyCreateCountAsync();

        // 강의 이외에 추가적인 API가 필요하다면 이곳에 기록(예를 들어, 시작일부터 종료일까지의 데이터 검색)
        // ...

        Task<ArticleSet<TextMessageModel, int>> GetAllAsync<TParentIdentifier>(int pageIndex, int pageSize, string searchField, string searchQuery, string sortOrder, TParentIdentifier parentIdentifier, int textMessageType);

        /// <summary>
        /// 특정 Vendor의 모든 연락처 조회
        /// </summary>
        Task<List<ContactModelForTextMessage>> GetAllContactsByVendorId(long vendorId);

        /// <summary>
        /// 특정 Employee의 모든 전화번호(Primary, Secondary) 조회
        /// </summary>
        Task<List<string>> GetAllEmployeePhoneNumber(long employeeId);
    }
}
