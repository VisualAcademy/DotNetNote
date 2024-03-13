using System.Collections.Generic;
using System.Linq;
using Dul.Data;

namespace DotNetNote.Models;

/// <summary>
/// Twelve 리포지토리 클래스
/// </summary>
public class TwelveRepository(DashboardContext context) : ITwelveRepository
{
    /// <summary>
    /// ParentId에 해당하는 1~12까지의 값을 가지는 12개 컬렉션 반환
    /// </summary>
    public List<Twelve> Seed(int parentId)
    {
        //// [1] 기존 데이터 지우기
        //_context.Database.ExecuteSqlCommand($"Delete Twelves Where ParentId = {parentId}");
        //_context.SaveChanges();
        // [2] 기존 ParentId에 해당하는 모든 레코드 삭제
        context.Twelves.Where(p => p.ParentId == parentId).ToList().ForEach(p => context.Twelves.Remove(p));
        context.SaveChanges();
                   
        List<Twelve> twelves = new List<Twelve>();

        for (int i = 1; i <= 12; i++)
        {
            twelves.Add(new Twelve { ParentId = parentId, MonthNumber = i });
        }

        context.Twelves.AddRange(twelves); // 데이터 추가(12개)
        context.SaveChanges(); // 업데이트

        return twelves;
    }

    /// <summary>
    /// 조건에 맞는 데이터가 있으면 업데이트 그렇지 않으면 입력 
    /// </summary>
    public void SaveOrUpdateProfit(int parentId, int monthNumber, int profit)
    {
        var twelve = context.Twelves.Where(t => t.ParentId == parentId && t.MonthNumber == monthNumber).SingleOrDefault();
        if (twelve != null)
        {
            // Update
            twelve.Profit = profit;
            context.Entry(twelve).State = Microsoft.EntityFrameworkCore.EntityState.Modified; // 수정 상태로 변경
            context.SaveChanges();
        }
        else
        {
            // Insert
            var t = new Twelve() { ParentId = parentId, MonthNumber = monthNumber, Profit = profit };
            context.Twelves.Add(t);
            context.SaveChanges(); 
        }
    }

    /// <summary>
    /// 특정 부모에 해당하는 12개 레코드를 반환 
    /// </summary>
    public List<Twelve> GetTwelves(int parentId)
    {
        var twelves = context.Twelves.Where(t => t.ParentId == parentId).OrderBy(t => t.MonthNumber).ToList(); 

        return twelves;
    }

    public Twelve Add(Twelve model) => throw new System.NotImplementedException();

    public Twelve Browse(int id) => throw new System.NotImplementedException();

    public bool Delete(int id)
    {
        throw new System.NotImplementedException();
    }

    public bool Edit(Twelve model)
    {
        throw new System.NotImplementedException();
    }

    public int Has()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Twelve> Ordering(OrderOption orderOption)
    {
        throw new System.NotImplementedException();
    }

    public List<Twelve> Paging(int pageNumber, int pageSize)
    {
        throw new System.NotImplementedException();
    }

    public List<Twelve> Read()
    {
        throw new System.NotImplementedException();
    }

    public List<Twelve> Search(string query)
    {
        throw new System.NotImplementedException();
    }
}
