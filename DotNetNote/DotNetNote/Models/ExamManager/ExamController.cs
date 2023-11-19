using DotNetNote.Models;
using DotNetNote.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class ExamController : Controller
    {
        private ExamService _examService; 

        public ExamController()
        {
            _examService = new ExamService();
        }

        public ActionResult Publish()
        {
            var exam = _examService.GetExam();
            return View(exam); 
        }

        public ActionResult Grade(Exam exam)
        {
            var grade =_examService.Grade(exam); 
            return View(grade); 
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnotherPublish()
        {
            var exam = _examService.GetExam(); 
            return View(exam); 
        }

        public ActionResult PublishQuiz()
        {
            var question = _examService.GetExam().Questions[0];
            return View(question); 
        }

        public ActionResult GradeQuiz(Question question)
        {
            question = _examService.GetExam().Questions[0];
            return View(question); 
        }

        public ActionResult GradeAnother(Exam exam)
        {
            return View(); 
        }

    }
}
