using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DotNetNote.Models.StudentManager
{
    /// <summary>
    /// Student 모델 클래스
    /// </summary>
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Student 리포지토리 클래스 
    /// </summary>
    public class StudentRepository
    {
        public List<Student> GetAllInMemory()
        {
            List<Student> students = new List<Student>();
            students.Add(new Student { StudentId = 1, Name = "홍길동" });
            students.Add(new Student { StudentId = 2, Name = "백두산" });
            return students; 
        }
    }

    /// <summary>
    /// Student Web API 서비스 클래스 
    /// </summary>
    [Route("api/[controller]")]
    public class StudentServiceController : Controller
    {
        private StudentRepository repo;

        public StudentServiceController()
        {
            repo = new StudentRepository();
        }

        [HttpGet("")]
        public IEnumerable<Student> Get()
        {
            return repo.GetAllInMemory().AsEnumerable();
        }

        [HttpGet("{id}")]
        public Student GetById(int id)
        {
            // 데이터 조회
            Student student = repo.GetAllInMemory()[id];
            if (student == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }
            return student;
        }
    }
}
