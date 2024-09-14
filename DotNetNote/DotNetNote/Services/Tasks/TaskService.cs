using DotNetNote.Records;

namespace DotNetNote.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly List<TodoRecord> _todos = new();

        public TodoRecord? GetTodoById(int id)
        {
            return _todos.SingleOrDefault(t => t.Id == id);
        }

        public List<TodoRecord> GetTodos()
        {
            return _todos;
        }

        public void DeleteTodoById(int id)
        {
            _todos.RemoveAll(t => t.Id == id);
        }

        public TodoRecord AddTodo(TodoRecord task)
        {
            _todos.Add(task);
            return task;
        }
    }

}
