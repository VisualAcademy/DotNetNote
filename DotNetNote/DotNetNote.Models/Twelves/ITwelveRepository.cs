using System.Collections.Generic;
using Dul.Data;

namespace DotNetNote.Models;

public interface ITwelveRepository : IBreadShop<Twelve>
{
    List<Twelve> Seed(int parentId);
    void SaveOrUpdateProfit(int parentId, int monthNumber, int profit);
    List<Twelve> GetTwelves(int parentId);
}
