using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DotNetNote.Models;

namespace DotNetNote.Controllers
{
    [Produces("application/json")]
    [Route("api/TechesApi")]
    public class TechesApiController : Controller
    {
        private ITechRepository _repo;

        // 의존성 주입: ITechRepository의 인스턴스를 TechRepository의 인스턴스로
        public TechesApiController(ITechRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<Tech> GetTech()
        {
            return _repo.GetTechs();
        }

        [HttpPost]
        public Tech PostTech([FromBody] Tech tech)
        {
            _repo.AddTech(tech);
            return tech;
        }
    }
}
