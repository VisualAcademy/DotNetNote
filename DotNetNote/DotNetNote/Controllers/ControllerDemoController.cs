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

    public string StringAction() => "String�� ��ȯ�ϴ� �׼� �޼���";

    public DateTime DateTimeAction() => DateTime.Now;

    public IActionResult DefaultAction() => View(); // ��Ʈ�ѷ�/�׼Ǹ޼���
}
