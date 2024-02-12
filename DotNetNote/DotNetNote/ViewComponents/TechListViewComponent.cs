namespace DotNetNote.ViewComponents;

public class TechListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var techLists = new List<Tech>()
        {
            new() { Id = 1, Title = "ASP.NET Core" },
            new Tech { Id = 2, Title = "Bootstrap" },
            new() { Id = 3, Title = "C#" },
            new Tech { Id = 4, Title = "Dapper" },
            new() { Id = 5, Title = "Azure" },
            new Tech { Id = 6, Title = "jQuery" },
            new() { Id = 7, Title = "Angular" }
        };

        return View(techLists);
    }
}
