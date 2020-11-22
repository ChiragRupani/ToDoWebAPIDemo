using System.Collections.Generic;
using ToDoWebAPI.DBContext;
using ToDoWebAPI.Models;

namespace ToDoWebAPI.IntegrationTests
{
    public class Utilities
    {
        public static void InitializeDbForTests(ToDoContext db)
        {
            var toDoList = getToDos();
            db.ToDo.AddRange(toDoList);
            db.SaveChanges();
        }

        public static void ReInitializeDbForTests(ToDoContext db)
        {
            db.RemoveRange(db.ToDo);
            InitializeDbForTests(db);
        }

        public static List<ToDo> getToDos()
        {
            return new List<ToDo>
            {
                new() { ID = 1, Title="First Wish", IsCompleted = false},
                new() { ID = 2, Title="Second Wish", IsCompleted = true},
                new() { ID = 3, Title="Test Wish", IsCompleted = true},
            };
        }
    }
}
