// ASP.NET Core Fundamentals 강의 - RecruitManager(모집 관리자) 강의 소스
using DotNetNote.Models.RecruitManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetNote.Controllers.RecruitManager
{
    [Authorize] // 로그인된 사용자만 테스트 가능 
    public class RecruitManagerController : Controller
    {
        private IRecruitSettingRepository _repo;

        public RecruitManagerController(IRecruitSettingRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// 기본 페이지 
        /// </summary>
        public IActionResult Index()
        {
            //return View();
            return View("~/Views/_MiniProjects/RecruitManager/Index.cshtml");
        }

        #region 모집 추가
        [HttpGet]
        public IActionResult RecruitSettingCreate()
        {
            //return View();
            return View("~/Views/_MiniProjects/RecruitManager/RecruitSettingCreate.cshtml");
        }

        [HttpPost]
        public IActionResult RecruitSettingCreate(RecruitSetting model)
        {
            // 정상적인 데이터인지 확인
            if (ModelState.IsValid)
            {
                // 실제 데이터베이스 저장
                _repo.Add(model);

                return RedirectToAction(nameof(RecruitSettingList));
            }

            //return View(model);
            return View("~/Views/_MiniProjects/RecruitManager/RecruitSettingCreate.cshtml", model);
        } 
        #endregion

        /// <summary>
        /// 모집 리스트
        /// </summary>
        public async Task<IActionResult> RecruitSettingList()
        {
            // 전체 레코드
            //var recruits = _repo.GetAll(); // 동기 방식
            var recruits = await _repo.GetAllAsync(); // 비동기 방식 

            //return View(recruits); 
            return View("~/Views/_MiniProjects/RecruitManager/RecruitSettingList.cshtml", recruits);
        }

        /// <summary>
        /// 모집 상세
        /// </summary>
        public IActionResult RecruitSettingDetail(int id)
        {
            ViewData["Id"] = id.ToString();

            var recruit = _repo.GetById(id); 

            //return View(recruit);
            return View("~/Views/_MiniProjects/RecruitManager/RecruitSettingDetail.cshtml", recruit);
        }

        /// <summary>
        /// 상세 데이터 수정 또는 삭제
        /// </summary>
        [HttpPost]
        public IActionResult RecruitSettingEditOrDelete(RecruitSetting model, string action)
        {
            if (action == "update")
            {
                // 데이터 수정
                if (ModelState.IsValid)
                {
                    _repo.Update(model);
                }

                // 수정 후 상세 보기 페이지로 이동 
                return RedirectToAction(
                    nameof(RecruitSettingDetail), new { Id = model.Id });
            }
            else
            {
                // 데이터 삭제
                if (ModelState.IsValid)
                {
                    // 히든 필드에 들어있는 Id 값에 해당하는 레코드 삭제
                    _repo.Remove(model.Id);
                }

                // 삭제 후 리스트 페이지로 이동 
                return RedirectToAction(nameof(RecruitSettingList));
            }
        }

        // [FromQuery] 특성으로,
        // BoardView?BoardName=Recruit&BoardNum=11 형태의 쿼리스트링 받기 
        [HttpGet]
        public IActionResult BoardView([FromQuery]string boardName, [FromQuery]int boardNum)
        {
            // [FromQuery] 특성 사용
            ViewData["BoardName"] = boardName;
            // HttpContext.Request.Query[], HttpContext.Request.Form[] 사용
            ViewData["BoardNum"] = HttpContext.Request.Query["BoardNum"];

            // 현재 게시판 관련된 모집 정보가 설정이 되어 있는지 확인
            ViewBag.IsRecruitSettings =
                _repo.IsRecruitSettings(boardName, boardNum);

            // 종료된 모집인지 확인
            //ViewBag.IsClosedRecruit = false;
            ViewBag.IsClosedRecruit = 
                _repo.IsClosedRecruit(boardName, boardNum);

            // 마감된 모집인지 확인
            ViewBag.IsFinishedRecruit =
                _repo.IsFinishedRecruit(boardName, boardNum);


            //return View();
            return View("~/Views/_MiniProjects/RecruitManager/BoardView.cshtml");
        }   
    }
}
