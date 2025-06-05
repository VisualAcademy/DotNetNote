using Azunt.NoteManagement;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Components.Pages.Notes.Apis;

[ApiController]
[Route("api/[controller]")]
public class NoteApiController : ControllerBase
{
    private readonly INoteRepository _noteRepository;

    public NoteApiController(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    // GET: api/NoteApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes()
    {
        var notes = await _noteRepository.GetAllAsync();

        var result = notes.Select(note => new NoteDto
        {
            Id = note.Id,
            Name = note.Name,
            Category = note.Category,
            Created = note.Created,
            CreatedBy = note.CreatedBy
        });

        return Ok(result);
    }

    // GET: api/NoteApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> GetNote(long id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        if (note == null || note.IsDeleted)
        {
            return NotFound();
        }

        var dto = new NoteDto
        {
            Id = note.Id,
            Name = note.Name,
            Category = note.Category,
            Created = note.Created,
            CreatedBy = note.CreatedBy
        };

        return Ok(dto);
    }

    // POST: api/NoteApi
    [HttpPost]
    public async Task<ActionResult<NoteDto>> PostNote(NoteDto dto)
    {
        var model = new Note
        {
            Name = dto.Name,
            Category = dto.Category,
            CreatedBy = dto.CreatedBy,
            Created = DateTimeOffset.UtcNow,
            Active = true,
            IsDeleted = false,
            DisplayOrder = 0,
        };

        var result = await _noteRepository.AddAsync(model);

        return CreatedAtAction(nameof(GetNote), new { id = result.Id }, new NoteDto
        {
            Id = result.Id,
            Name = result.Name,
            Category = result.Category,
            Created = result.Created,
            CreatedBy = result.CreatedBy
        });
    }

    // PUT: api/NoteApi/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutNote(long id, NoteDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var model = await _noteRepository.GetByIdAsync(id);
        if (model == null || model.IsDeleted) return NotFound();

        model.Name = dto.Name;
        model.Category = dto.Category;

        await _noteRepository.UpdateAsync(model);
        return NoContent();
    }

    // DELETE: api/NoteApi/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(long id)
    {
        var success = await _noteRepository.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}