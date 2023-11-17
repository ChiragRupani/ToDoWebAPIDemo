namespace ToDoWebAPI.Models;

public record ToDo
{
    public int ID { get; set; }
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
}
