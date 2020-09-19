using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoWebAPI.Models;

namespace ToDoWebAPI.DBContext
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ToDo>()
                .Property(e => e.ID)
                .ValueGeneratedOnAdd();

        }

        public async Task EnsureInitialToDoAsync(string filePath)
        {
            bool hasRecords = await ToDo.AnyAsync();
            if (!hasRecords && !string.IsNullOrEmpty(filePath))
            {
                string jsonData = await File.ReadAllTextAsync(filePath);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    List<ToDo> result = JsonSerializer.Deserialize<List<ToDo>>(jsonData);
                    await ToDo.AddRangeAsync(result);
                    await this.SaveChangesAsync();
                }
            }
        }

        public DbSet<ToDo> ToDo { get; set; }
    }
}
