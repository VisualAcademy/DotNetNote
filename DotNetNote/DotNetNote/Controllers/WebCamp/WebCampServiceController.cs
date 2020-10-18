using DotNetNote.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DotNetNote.Controllers
{
    [Route("api/[controller]")]
    public class WebCampServiceController : Controller
    {
        [HttpGet]
        public IEnumerable<WebCampSpeaker> Get()
        {
            var lst = new List<WebCampSpeaker>();

            lst.Add(new WebCampSpeaker {
                Id = 1, Name = "박용준 MVP", Photo = "박용준 MVP",
                Title = "MVP",
                Description = "데브렉 전임강사" });
            lst.Add(new WebCampSpeaker {
                Id = 2, Name = "김태영 부장", Photo = "김태영 부장",
                Title = "Microsoft",
                Description = "한국마이크로소프트 DX, Technical Evangelist" });
            lst.Add(new WebCampSpeaker {
                Id = 3, Name = "김명신 부장", Photo = "김명신 부장",
                Title = "Microsoft",
                Description = "한국마이크로소프트 DX, Technical Evangelist" });
            lst.Add(new WebCampSpeaker {
                Id = 4, Name = "한상훈 MVP", Photo = "한상훈 MVP",
                Title = "MVP",
                Description = "넥슨 개발자" });
            lst.Add(new WebCampSpeaker {
                Id = 5, Name = "성지용 부장", Photo = "성지용 부장",
                Title = "Microsoft",
                Description = "한국마이크로소프트 DX, Technical Evangelist" });
            
            return lst;        
        }
    }
}
