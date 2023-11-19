using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Components;

/// <summary>
/// Todo 컴포넌트
/// </summary>
public class TodoComponent
{
    // Empty
}

/// <summary>
/// Todo 모델 클래스: Todos 테이블과 일대일로 매핑
/// </summary>
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsDone { get; set; } = false; // IsComplete 속성명도 많이 사용
}

/// <summary>
/// TodoItem 모델 클래스
/// </summary>
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsDone { get; set; } = false;
}

/// <summary>
/// Todo 컨텍스트 클래스
/// </summary>
public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
        // Empty
    }

    public DbSet<Todo> Todos { get; set; }
}

/// <summary>
/// 리포지터리 인터페이스
/// </summary>
public interface ITodoRepository
{

}

/// <summary>
/// 리포지터리 클래스
/// </summary>
public class TodoRepository : ITodoRepository
{

}

/// <summary>
/// MVC 컨트롤러
/// </summary>
public class TodoController : Controller
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context) => _context = context;

    // GET: Todo
    public async Task<IActionResult> Index() => View(await _context.Todos.ToListAsync());

    // GET: Todo/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var todo = await _context.Todos
            .SingleOrDefaultAsync(m => m.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        return View(todo);
    }

    // GET: Todo/Create
    public IActionResult Create() => View();

    // POST: Todo/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,IsDone")] Todo todo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(todo);
    }

    // GET: Todo/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var todo = await _context.Todos.SingleOrDefaultAsync(m => m.Id == id);
        if (todo == null)
        {
            return NotFound();
        }
        return View(todo);
    }

    // POST: Todo/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsDone")] Todo todo)
    {
        if (id != todo.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(todo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(todo);
    }

    // GET: Todo/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var todo = await _context.Todos
            .SingleOrDefaultAsync(m => m.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        return View(todo);
    }

    // POST: Todo/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var todo = await _context.Todos.SingleOrDefaultAsync(m => m.Id == id);
        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TodoExists(int id) => _context.Todos.Any(e => e.Id == id);
}

/// <summary>
/// Web API 컨트롤러
/// </summary>
[Produces("application/json")]
[Route("api/Todos")]
public class TodosController : Controller
{
    private readonly TodoContext _context;

    public TodosController(TodoContext context) => _context = context;

    // GET: api/Todos
    [HttpGet]
    public IEnumerable<Todo> GetTodos() => _context.Todos;

    // GET: api/Todos/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodo([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var todo = await _context.Todos.SingleOrDefaultAsync(m => m.Id == id);

        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    // PUT: api/Todos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodo([FromRoute] int id, [FromBody] Todo todo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != todo.Id)
        {
            return BadRequest();
        }

        _context.Entry(todo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoExists(id))
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

    // POST: api/Todos
    [HttpPost]
    public async Task<IActionResult> PostTodo([FromBody] Todo todo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
    }

    // DELETE: api/Todos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var todo = await _context.Todos.SingleOrDefaultAsync(m => m.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return Ok(todo);
    }

    private bool TodoExists(int id) => _context.Todos.Any(e => e.Id == id);
}
