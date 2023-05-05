using Microsoft.EntityFrameworkCore;
using ToDoWebAPI.DBContext;
using ToDoWebAPI.Models;

namespace ToDoWebAPI.Repository;

public class ToDoRepository
{
    private readonly ToDoContext context;

    public ToDoRepository(IWebHostEnvironment environment, ToDoContext context)
    {
        this.context = context;
        string filePath = Path.Combine(environment.ContentRootPath, "Repository\\todos.json");
        context.EnsureInitialToDoAsync(filePath).GetAwaiter().GetResult();
    }

    public Task<List<ToDo>> GetToDosAsync()
    {
        return context.ToDo.ToListAsync();
    }

    public async Task AddToDoAsync(ToDo model)
    {
        context.ToDo.Add(model);
        await context.SaveChangesAsync();
    }

    public ValueTask<ToDo?> FindAsync(int ID)
    {
        var result = context.FindAsync<ToDo>(ID);
        return result;
    }

    public async Task UpdateAsync(ToDo toDo)
    {
        context.Entry(toDo).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ToDo todo)
    {
        context.ToDo.Remove(todo);
        await context.SaveChangesAsync();
    }

    internal bool ToDoExists(int ID)
    {
        return context.ToDo.Any(e => e.ID == ID);
    }
}
