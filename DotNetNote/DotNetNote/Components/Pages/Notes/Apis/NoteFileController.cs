using Microsoft.AspNetCore.Mvc;
using Azunt.NoteManagement;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Azunt.Apis.Notes
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteFileController : ControllerBase
    {
        private readonly INoteStorageService _storageService;
        private readonly INoteRepository _noteRepository;

        public NoteFileController(INoteStorageService storageService, INoteRepository noteRepository)
        {
            _storageService = storageService;
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// 파일명을 이용한 직접 다운로드
        /// GET /api/NoteFile/{fileName}
        /// </summary>
        [HttpGet("{fileName}")]
        public async Task<IActionResult> DownloadByFileName(string fileName)
        {
            try
            {
                var stream = await _storageService.DownloadAsync(fileName);
                return File(stream, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound($"File not found: {fileName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Download error: {ex.Message}");
            }
        }

        /// <summary>
        /// Note ID를 통해 파일명을 조회하여 다운로드
        /// GET /api/NoteFile/ById/{id}
        /// </summary>
        [HttpGet("ById/{id}")]
        public async Task<IActionResult> DownloadById(long id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null)
            {
                return NotFound($"Note with Id {id} not found.");
            }

            if (string.IsNullOrEmpty(note.FileName))
            {
                return NotFound($"No file attached to note with Id {id}.");
            }

            try
            {
                var stream = await _storageService.DownloadAsync(note.FileName);
                return File(stream, "application/octet-stream", note.FileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound($"File not found: {note.FileName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Download error: {ex.Message}");
            }
        }
    }
}