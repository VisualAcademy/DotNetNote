namespace DotNetNote.Components
{
    public class TakePhotoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}