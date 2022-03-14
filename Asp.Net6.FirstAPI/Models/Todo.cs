namespace Todo.Models
{
    public class TodoModel
    {
        public TodoModel(int id, string title, bool done, DateTime createdAt)
        {
            Id = id;
            Title = title;
            Done = done;
            CreatedAt = createdAt;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
