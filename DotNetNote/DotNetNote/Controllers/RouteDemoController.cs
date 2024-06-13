namespace DotNetNote.Controllers;

// [Route()] 특성을 사용한 특성(어트리뷰트, 애트리뷰트) 라우팅
[Route("RouteDemo")]
public class RouteDemoController
{
    [Route(""), Route("Index")]
    public string Index() => "특성(어트리뷰트) 라우팅";
}
