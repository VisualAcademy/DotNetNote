using DotNetNote.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DotNetNote.Controllers
{
    public class UrlsServicesController : Controller
    {
        private readonly DotNetNoteContext _context;

        public UrlsServicesController(DotNetNoteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// URL 리스트 출력
        /// </summary>
        /// <param name="page">페이지 번호</param>
        /// <param name="keyword">검색 키워드</param>
        /// <returns>URL 목록</returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult UrlIndex(int page = 1, string keyword = "")
        {
            int cnt = 0;

            List<Url> urls;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                urls = _context.Urls.OrderByDescending(n => n.Id).Skip((page - 1) * 5).Take(5).ToList();
                cnt = _context.Urls.Count();
            }
            else
            {
                urls = _context.Urls.OrderByDescending(n => n.Id).Where(n => n.SiteUrl.Contains(keyword)).Skip((page - 1) * 5).Take(5).ToList();
                cnt = _context.Urls.Where(n => EF.Functions.Like(n.SiteUrl, "N%" + keyword + "%") || n.SiteUrl.Contains(keyword)).Count();
            }

            return Json(new
            {
                list = urls.Select(x => new
                {
                    id = x.Id,
                    siteUrl = x.SiteUrl,
                    created = x.Created.ToString("yyyy.MM.dd"),
                    content = x.Content,
                    userName = x.UserName
                }),
                count = cnt
            });
        }

        /// <summary>
        /// 로그인 페이지의 공지사항 상세에 사용될 공지사항 정보 조회
        /// </summary>
        /// <param name="id">키</param>
        /// <param name="keyword">검색 키워드</param>
        /// <returns>공지사항 정보</returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult UrlDetails(int id, string keyword = "")
        {
            var articleBase = _context.Urls.Where(n => n.Id == id).SingleOrDefault();

            Url prev = new Url();
            Url next = new Url();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                prev = _context.Urls.Where(n => n.Id < id).OrderByDescending(n => n.Id).FirstOrDefault(); // 이전 
                next = _context.Urls.Where(n => n.Id > id).OrderBy(n => n.Id).FirstOrDefault(); // 다음 
            }
            else
            {
                prev = _context.Urls.Where(n => n.Id < id && n.SiteUrl.Contains(keyword)).OrderByDescending(n => n.Id).FirstOrDefault(); // 이전 
                next = _context.Urls.Where(n => n.Id > id && n.SiteUrl.Contains(keyword)).OrderBy(n => n.Id).FirstOrDefault(); // 다음 
            }

            return Json(new
            {
                siteUrl = articleBase.SiteUrl,
                userName = articleBase.UserName,
                created = articleBase.Created.ToString("yyyy.MM.dd"),
                content = articleBase.Content,
                prevNum = (prev != null ? prev.Id : -1),
                prevTitle = (prev != null ? prev.SiteUrl : ""),
                nextNum = (next != null ? next.Id : -1),
                nextTitle = (next != null ? next.SiteUrl : "")
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DeleteUrlById(int id)
        {
            var deleteArticle = _context.Urls.Where(n => n.Id == id).SingleOrDefault();
            _context.Entry(deleteArticle).State = EntityState.Deleted;
            _context.SaveChanges();

            return Json(new
            {
                message = "DELETED"
            });
        }

        /// <summary>
        /// 새로운 URL을 등록하고 그 결과를 반환합니다.
        /// </summary>
        /// <param name="url">URL 개체의 인스턴스</param>
        /// <returns>등록결과</returns>
        [HttpPost]
        [Route("api/UrlsServices/PostUrl")]
        [AllowAnonymous]
        public IActionResult PostUrl([FromBody]Url url)
        {

            url.Created = DateTime.Now; 

            if (ModelState.IsValid)
            {
                if (url.Id == -1)
                {
                    // 입력
                    url.Id = 0;


                    _context.Urls.Add(url);
                    _context.SaveChanges(); 

                    // 201 응답을 생성합니다.
                    var response = new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new StringContent(url.Id.ToString())
                    };

                    return Created("", "");

                }
                else
                {
                    _context.Entry(url).State = EntityState.Modified;
                    int r = _context.SaveChanges();

                    if (r < 1)
                    {
                        return Json(new
                        {
                            modified = false,
                            msg = "수정되지 않았습니다."
                        });
                    }
                    return Created("", "");
                }
            }
            else
            {
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState); // ASP.NET 
                HttpRequestMessage request = new HttpRequestMessage();
                //return request.CreateResponse(HttpStatusCode.BadRequest); // ASP.NET Core
                return BadRequest(ModelState);
            }
        }
    }
}
