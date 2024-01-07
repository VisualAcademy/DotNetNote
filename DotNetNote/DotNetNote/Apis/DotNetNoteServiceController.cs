using Microsoft.AspNetCore.Mvc;
using DotNetNote.Models.Notes;

namespace DotNetNote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotNetNoteServiceController : ControllerBase
    {
        private readonly INoteRepository _repository;

        public DotNetNoteServiceController(INoteRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("Delete/{id}")]
        public IActionResult DeletePost(int id, [FromBody] DeleteViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { Message = "비밀번호가 제공되지 않았습니다." });
            }

            if (_repository.DeleteNote(id, model.Password) > 0)
            {
                return Ok(new { Message = "데이터가 삭제되었습니다." });
            }
            else
            {
                return BadRequest(new { Message = "삭제되지 않았습니다. 비밀번호를 확인하세요." });
            }
        }
    }

    public record DeleteViewModel(string Password);
}
