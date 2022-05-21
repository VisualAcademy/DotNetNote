using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class DulTestController : Controller
{
    public IActionResult Index() => View();

    public string IsPhotoTest()
    {
        string file = "a.png";
        return Dul.BoardLibrary.IsPhoto(file).ToString(); // True
    }
}
