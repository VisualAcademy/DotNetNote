namespace DotNetNote.ViewComponents;

public class SiteListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var siteLists = new List<Site>() {
            new Site { Id = 1, Title = "길벗",
                Url = "http://www.gilbut.co.kr",
                Description = "C# 교과서 및 ASP.NET & Core를 다루는 기술 책 출간" },
            new Site { Id = 2, Title = "데브렉",
                Url = "http://www.devlec.com",
                Description = "DotNetNote 사이트 제작 관련 동영상 강의 제공" },
            new Site { Id = 3, Title = "Taeyo.NET",
                Url = "http://www.taeyo.net",
                Description = "ASP.NET Core 강좌 제공" },
            new Site { Id = 4, Title = "ASP.NET Korea User Group",
                Url = "https://www.facebook.com/groups/AspxKorea/",
                Description = "ASP.NET 한국 사용자 그룹" },
            new Site { Id = 5, Title = "닷넷코리아",
                Url = "http://www.dotnetkorea.com",
                Description = "박용준 MVP 개인 홈페이지" },
            new Site { Id = 6, Title = "비주얼아카데미",
                Url = "https://www.youtube.com/user/visualacademy",
                Description = "박용준 MVP 개인 유튜브 채널" },
            new Site { Id = 7, Title = "ASP.NET",
                Url = "http://www.asp.net",
                Description = "ASP.NET 공식 사이트" }
        };

        return View(siteLists);
    }
}
