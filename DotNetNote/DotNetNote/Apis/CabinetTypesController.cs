﻿#nullable disable
namespace DotNetNote.Apis;

[Route("api/[controller]")]
[ApiController]
public class CabinetTypesController(ApplicationDbContext context, ILogger<CabinetTypesController> logger) : ControllerBase
{
    // GET: api/CabinetTypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CabinetType>>> GetCabinetTypes() => await context.CabinetTypes.ToListAsync();

    // GET: api/CabinetTypes/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<CabinetType>> GetCabinetType(long id)
    {
        var cabinetType = await context.CabinetTypes.FindAsync(id);

        if (cabinetType == null)
        {
            return NotFound();
        }

        return cabinetType;
    }

    // PUT: api/CabinetTypes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCabinetType(long id, CabinetType cabinetType)
    {
        if (id != cabinetType.Id)
        {
            return BadRequest();
        }

        context.Entry(cabinetType).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CabinetTypeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/CabinetTypes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CabinetType>> PostCabinetType(CabinetType cabinetType)
    {
        context.CabinetTypes.Add(cabinetType);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCabinetType", new { id = cabinetType.Id }, cabinetType);
    }

    // DELETE: api/CabinetTypes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCabinetType(long id)
    {
        var cabinetType = await context.CabinetTypes.FindAsync(id);
        if (cabinetType == null)
        {
            return NotFound();
        }

        context.CabinetTypes.Remove(cabinetType);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool CabinetTypeExists(long id) => context.CabinetTypes.Any(e => e.Id == id);

    // 페이징
    // GET api/Entries/Page/1/10
    [HttpGet("Page/{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<CabinetType>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            // 페이지 번호는 1, 2, 3 사용, 리포지토리에서는 0, 1, 2 사용
            int pageIndex = pageNumber > 0 ? pageNumber - 1 : 0;

            var totalRecords = await context.CabinetTypes.CountAsync();
            var models = await context.CabinetTypes.OrderByDescending(n => n.Id).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            if (models == null)
            {
                return NotFound($"아무런 데이터가 없습니다.");
            }

            // 응답 헤더 설정을 위한 인덱서 사용. 인덱서는 키가 이미 존재하면 값을 업데이트하고, 존재하지 않으면 새 항목을 추가한다. 이로 인해 중복 키 오류를 방지하고 코드의 유연성을 높인다.
            Response.Headers["X-TotalRecordCount"] = totalRecords.ToString();
            Response.Headers["Access-Control-Expose-Headers"] = "X-TotalRecordCount";

            //return Ok(resultSet.Records);
            var ʘ‿ʘ = models; // 재미를 위해서 
            return Ok(ʘ‿ʘ); // Look of Approval
        }
        catch (Exception ಠ_ಠ) // Look of Disapproval
        {
            logger?.LogError($"ERROR({nameof(GetAll)}): {ಠ_ಠ.Message}");
            return BadRequest();
        }
    }

    // 검색
    // GET api/Entries/Page/1/10
    [HttpGet("Search/{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<CabinetType>>> GetAll(int pageNumber = 1, int pageSize = 10, string searchQuery = "")
    {
        try
        {
            // 페이지 번호는 1, 2, 3 사용, 리포지토리에서는 0, 1, 2 사용
            int pageIndex = pageNumber > 0 ? pageNumber - 1 : 0;

            var totalRecords = await context.CabinetTypes.Where(s => s.Identification.Contains(searchQuery)).CountAsync();
            var models = await context.CabinetTypes.Where(s => s.Identification.Contains(searchQuery)).OrderByDescending(n => n.Id).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            if (models == null)
            {
                return NotFound($"아무런 데이터가 없습니다.");
            }

            //// 응답 헤더에 총 레코드 수를 담아서 출력
            //Response.Headers.Add("X-TotalRecordCount", totalRecords.ToString());
            //Response.Headers.Add("Access-Control-Expose-Headers", "X-TotalRecordCount");
            // 응답 헤더에 총 레코드 수를 담아서 출력 (인덱서 사용)
            Response.Headers["X-TotalRecordCount"] = totalRecords.ToString();
            Response.Headers["Access-Control-Expose-Headers"] = "X-TotalRecordCount";

            //return Ok(resultSet.Records);
            var ʘ‿ʘ = models; // 재미를 위해서 
            return Ok(ʘ‿ʘ); // Look of Approval
        }
        catch (Exception ಠ_ಠ) // Look of Disapproval
        {
            logger?.LogError($"ERROR({nameof(GetAll)}): {ಠ_ಠ.Message}");
            return BadRequest();
        }
    }
}
