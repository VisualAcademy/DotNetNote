using TodoApi.Models;
using TodoItem = TodoApi.Models.TodoItem;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController(TodoApi.Models.TodoContext context) : ControllerBase
{
    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems() => await context.TodoItems.Select(x => ItemToDTO(x)).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return ItemToDTO(todoItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
    {
        if (id != todoItemDTO.Id)
        {
            return BadRequest();
        }

        var todoItem = await context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.Name = todoItemDTO.Name;
        todoItem.IsComplete = todoItemDTO.IsComplete;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
    {
        var todoItem = new TodoItem
        {
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name
        };

        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        context.TodoItems.Remove(todoItem);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id) =>
         context.TodoItems.Any(e => e.Id == id);

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        new()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
}
