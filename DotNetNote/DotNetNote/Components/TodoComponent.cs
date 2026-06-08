using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; } = false; // IsComplete 속성명도 많이 사용
}

/// <summary>
/// TodoItem 모델 클래스
/// </summary>
public class TodoItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; } = false;
}

/// <summary>
/// Todo 컨텍스트 클래스
/// </summary>
public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; } = default!;
}

/// <summary>
/// 리포지터리 인터페이스
/// </summary>
public interface ITodoRepository
{
    // 향후 Todo 관련 CRUD 메서드를 정의할 수 있습니다.
}

/// <summary>
/// 리포지터리 클래스
/// </summary>
public class TodoRepository : ITodoRepository
{
    // 향후 ITodoRepository 구현 코드를 작성할 수 있습니다.
}

/// <summary>
/// MVC 컨트롤러
/// </summary>
public class TodoController(TodoContext context) : Controller
{
    // GET: Todo
    public async Task<IActionResult> Index()
    {
        var todos = await context.Todos.ToListAsync();

        return View(todos);
    }

    // GET: Todo/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var todo = await context.Todos
            .SingleOrDefaultAsync(m => m.Id == id.Value);

        if (todo is null)
        {
            return NotFound();
        }

        return View(todo);
    }

    // GET: Todo/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Todo/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,IsDone")] Todo todo)
    {
        if (!ModelState.IsValid)
        {
            return View(todo);
        }

        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Todo/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var todo = await context.Todos
            .SingleOrDefaultAsync(m => m.Id == id.Value);

        if (todo is null)
        {
            return NotFound();
        }

        return View(todo);
    }

    // POST: Todo/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsDone")] Todo todo)
    {
        if (id != todo.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(todo);
        }

        try
        {
            context.Todos.Update(todo);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoExists(todo.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Todo/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var todo = await context.Todos
            .SingleOrDefaultAsync(m => m.Id == id.Value);

        if (todo is null)
        {
            return NotFound();
        }

        return View(todo);
    }

    // POST: Todo/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var todo = await context.Todos
            .SingleOrDefaultAsync(m => m.Id == id);

        if (todo is null)
        {
            return NotFound();
        }

        context.Todos.Remove(todo);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool TodoExists(int id)
    {
        return context.Todos.Any(e => e.Id == id);
    }
}

/// <summary>
/// Web API 컨트롤러
/// </summary>
[Produces("application/json")]
[Route("api/Todos")]
public class TodosController(TodoContext context) : Controller
{
    // GET: api/Todos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        var todos = await context.Todos.ToListAsync();

        return Ok(todos);
    }

    // GET: api/Todos/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTodo([FromRoute] int id)
    {
        var todo = await context.Todos
            .SingleOrDefaultAsync(m => m.Id == id);

        if (todo is null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    // PUT: api/Todos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutTodo([FromRoute] int id, [FromBody] Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        context.Entry(todo).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoExists(id))
            {
                return NotFound();
            }

            throw;
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

        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
    }

    // DELETE: api/Todos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodo([FromRoute] int id)
    {
        var todo = await context.Todos
            .SingleOrDefaultAsync(m => m.Id == id);

        if (todo is null)
        {
            return NotFound();
        }

        context.Todos.Remove(todo);
        await context.SaveChangesAsync();

        return Ok(todo);
    }

    private bool TodoExists(int id)
    {
        return context.Todos.Any(e => e.Id == id);
    }
}