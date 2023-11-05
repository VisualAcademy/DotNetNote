using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetNote.ViewComponents;

public class DataListViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string name)
    {
        var data = await GetByNameAsync(name);
        return View(data);
    }

    /// <summary>
    /// 비동기 메서드 
    /// </summary>
    private Task<IEnumerable<DataModel>> GetByNameAsync(string name) => Task.FromResult(GetByName(name));

    /// <summary>
    /// 동기 메서드
    /// </summary>
    private IEnumerable<DataModel> GetByName(string name)
    {
        DataService service = new DataService();
        return service.GetDataByName(name);
    }
}
