//[Angular 강의] Points 테이블부터 Point 컴포넌트까지 JSON 데이터를 읽어다가 앵귤러 뷰 컴포넌트에 출력하기 데모
using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetNote.Components;

public class PointComponent
{
    // Empty
    // Points 테이블부터 모델_리포지토리_컨트롤러_Web API를 거쳐_jQuery까지 전체 단계 사용하기
}

/// <summary>
/// Point 모델 클래스: Points 테이블과 일대일로 매핑
/// </summary>
public class Point
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public int TotalPoint { get; set; }
}

/// <summary>
/// PointLog 모델 클래스: PointLogs 테이블과 일대일로 매핑
/// </summary>
public class PointLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public int NewPoint { get; set; }
    public DateTimeOffset Created { get; set; }
}

/// <summary>
/// 포인트 상태 정보를 금, 은, 동으로 반환
/// </summary>
public class PointStatus
{
    public int Gold { get; set; }
    public int Silver { get; set; }
    public int Bronze { get; set; }
}

/// <summary>
/// 리포지토리 인터페이스
/// </summary>
public interface IPointRepository
{
    int GetTotalPointByUserId(int userId = 1234);
    PointStatus GetPointStatusByUser();
}

/// <summary>
/// 리포지토리 클래스
/// </summary>
public class PointRepository : IPointRepository
{
    public PointStatus GetPointStatusByUser()
    {
        throw new NotImplementedException();
    }

    public int GetTotalPointByUserId(int userId = 1234) =>
        //TODO: 실제 데이터베이스 연동하는 코드
        1234;
}

public class PointRepositoryInMemory : IPointRepository
{
    public PointStatus GetPointStatusByUser()
    {
        return new PointStatus() { Gold = 10, Silver = 123, Bronze = 345 };
    }

    public int GetTotalPointByUserId(int userId = 1234)
    {
        return 1234;
    }
}

public interface IPointLogRepository
{

}

public class PointLogRepository : IPointLogRepository
{

}

public class PointController : Controller
{
    private IPointRepository _repository;

    public PointController(IPointRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var myPoint = _repository.GetTotalPointByUserId();
        ViewBag.MyPoint = myPoint;
        return View();
    }
}

[Route("api/[controller]")]
public class PointServiceController : Controller
{
    private IPointRepository _repository;

    public PointServiceController(IPointRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [Route("")]
    public IActionResult Get()
    {
        var myPoint = _repository.GetTotalPointByUserId();
        var json = new { Point = myPoint };
        return Ok(json);
    }

    [HttpGet]
    [Route("{userId:int}")]
    public IActionResult Get(int userId)
    {
        var myPoint = _repository.GetTotalPointByUserId(userId);
        var json = new { Point = myPoint };
        return Ok(json);
    }
}

public class PointLogController : Controller
{
    public IActionResult Index() => View();
}

[Route("api/[controller]")]
public class PointLogServiceController : Controller
{
    [HttpGet]
    [Route("")]
    public IActionResult Get()
    {
        var json = new { Point = 2345 };
        return Ok(json);
    }
}

/// <summary>
/// 포인트 상태 정보를 반환하는 Web API
/// </summary>
[Route("api/[controller]")]
public class PointStatusController : Controller
{
    private IPointRepository _repository;

    public PointStatusController(IPointRepository repository) => _repository = repository;

    [HttpGet]
    [Route("")]
    public IActionResult Get()
    {
        var point = _repository.GetPointStatusByUser();
        return Ok(point);
    }
}
