namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string Secret { get; set; } = string.Empty;
    }

    public class TodoItemDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
    }
}