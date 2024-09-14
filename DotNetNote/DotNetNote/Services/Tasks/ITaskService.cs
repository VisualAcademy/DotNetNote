using DotNetNote.Records;

namespace DotNetNote.Services.Tasks
{
    public interface ITaskService
    {
        TodoRecord? GetTodoById(int id);
        List<TodoRecord> GetTodos();
        void DeleteTodoById(int id);
        TodoRecord AddTodo(TodoRecord task);
    }
}
