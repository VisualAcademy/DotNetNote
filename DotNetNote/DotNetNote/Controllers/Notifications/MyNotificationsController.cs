using DotNetNote.Models.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers;

public class MyNotificationsController : Controller
{
    private readonly IMyNotificationRepository _repository;

    public MyNotificationsController(IMyNotificationRepository repository)
    {
        _repository = repository;
    }

    #region MVC 액션 메서드
    public IActionResult Index() => View();

    public IActionResult MyNotification()
    {
        ViewBag.UserId = 1;

        return View();
    }

    public IActionResult MyNotificationWithModal()
    {
        ViewBag.UserId = 1;

        return View();
    }

    public IActionResult MyPage()
    {
        var userId = 1;

        ViewBag.UserId = userId;

        var noti = _repository.GetNotificationByUserid(userId);

        return View(noti);
    } 
    #endregion

    #region Web API
    [Route("api/IsNotification")]
    public bool IsNotification(int userId)
    {
        return _repository.IsNotification(userId);
    }

    [Route("api/CompleteNotification")]
    public bool CompleteNotification(int userId)
    {
        _repository.CompleteNotificationByUserid(userId);
        return true;
    } 
    #endregion
}
