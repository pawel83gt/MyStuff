namespace Razor.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public TodoCategory TodoCategory { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
