using System.Collections.Generic;

namespace DotNetNote.Models
{
    /// <summary>
    /// [3] 인터페이스
    /// Goods 테이블에 대한 CURD API 명세서 작성
    /// </summary>
    public interface IGoodsRepository
    {
        GoodsBase AddGoods(GoodsBase model);        // 입력: T Add(T model); 
        List<GoodsBase> GetAllGoods();              // 출력: List<T> GetAll(); 
        GoodsBase GetGoodsById(int goodsId);        // 상세: T GetById(int id); 
        GoodsBase UpdateGoods(GoodsBase model);     // 수정: T Update(T model); 
        void RemoveGoods(int id);                   // 삭제: void Remove(int id);

        //int GetTotalRecordCountGoods();             // 총 레코드 수
        GoodsSet GetAllGoodsWithPaging(int pageNumber = 1, int pageSize = 10); // 페이징
    }
}
