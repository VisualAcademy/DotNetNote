using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Models.StudentManager;

/// <summary>
/// [1] Student 모델 클래스
/// </summary>
public class Student
{
    public int StudentId { get; set; }

    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// [2] Student 리포지토리 클래스 - 인메모리
/// </summary>
public class StudentRepository
{
    public List<Student> GetAllInMemory()
    {
        return
        [
            new Student
            {
                StudentId = 1,
                Name = "홍길동"
            },
            new Student
            {
                StudentId = 2,
                Name = "백두산"
            }
        ];
    }
}

/// <summary>
/// [3] Student Web API 서비스 클래스
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StudentServiceController : ControllerBase
{
    private readonly StudentRepository _repository;

    public StudentServiceController()
    {
        _repository = new StudentRepository();
    }

    [HttpGet]
    public IEnumerable<Student> Get()
    {
        return _repository.GetAllInMemory();
    }

    [HttpGet("{id:int}")]
    public ActionResult<Student> GetById(int id)
    {
        var student = _repository
            .GetAllInMemory()
            .FirstOrDefault(student => student.StudentId == id);

        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }
}