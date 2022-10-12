using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class TagHelperDemoController : Controller
{
    public IActionResult Index() => View();

    /// <summary>
    /// environment �±� ���� ����ϱ�
    /// </summary>
    public IActionResult EnvironmentDemo() => View();

    /// <summary>
    /// ���� �±� ���ۿ� ���λ� ���̱�
    /// </summary>
    public IActionResult PrefixDemo()
    {
        return View();
    }

    /// <summary>
    /// ����� ���� �±� ���� �׽�Ʈ
    /// </summary>
    public IActionResult MyTagHelperDemo()
    {
        return View();
    }

    /// <summary>
    /// Ŀ���� �±� ����
    /// </summary>
    public IActionResult EmailLinkDemo()
    {
        return View();
    }

    /// <summary>
    /// ���н� �ð� ����� �±� ���� ��� �׽�Ʈ
    /// </summary>
    public IActionResult TagHelperDemo()
    {
        return View();
    }

    /// <summary>
    /// ����¡ ����
    /// </summary>
    public IActionResult PagingHelperDemo()
    {
        return View();
    }

    /// <summary>
    /// Cache �±� ���� 
    /// </summary>
    public IActionResult CacheDemo()
    {
        return View();
    }


    /// <summary>
    /// ��ũ�ٿ� ���
    /// </summary>
    public IActionResult MarkdownViewerDemo()
    {
        return View();
    }
}
