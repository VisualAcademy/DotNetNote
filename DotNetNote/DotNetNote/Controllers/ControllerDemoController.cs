using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.Controllers;

// ��Ʈ�ѷ�: Ŭ����
// /ControllerDemo/
public class ControllerDemoController : Controller
{
    // �׼�: �޼���
    // /ControllerDemo/Index
    public void Index()
    {
        // �ƹ��� ���� ������� ����
    }

    public string StringAction()
    {
        return "String�� ��ȯ�ϴ� �׼� �޼���";
    }

    public DateTime DateTimeAction()
    {
        return DateTime.Now;
    }

    public IActionResult DefaultAction()
    {
        return View(); // ��Ʈ�ѷ�/�׼Ǹ޼���
    }
}
