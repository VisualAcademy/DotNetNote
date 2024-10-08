namespace DotNetNote.Components.TextMessages;

public class AddTextMessageViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult(View());
    }
}
