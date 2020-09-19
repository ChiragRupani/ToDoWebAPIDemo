using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ToDoWebAPI.DBContext;
using ToDoWebAPI.Models;

namespace ToDoWebAPI.Repository
{
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

        public ValueTask<ToDo> FindAsync(int id)
        {
            var result = context.FindAsync<ToDo>(id);
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

        internal bool ToDoExists(int id)
        {
            return context.ToDo.Any(e => e.ID == id);
        }
    }
}
