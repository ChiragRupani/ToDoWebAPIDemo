namespace ToDoWebAPI.Models;

public record ToDo
{
    public int ID { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
