using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoWebAPI.Models;
using ToDoWebAPI.Repository;

namespace ToDoWebAPI.Controllers;

// Union types leveraging built-in HttpResults framework classes
public union GetToDosResult(Ok<List<ToDo>>);
public union GetToDoResult(Ok<ToDo>, NotFound);
public union PutToDoResult(NoContent, BadRequest, NotFound);
public union PostToDoResult(CreatedAtRoute<ToDo>);
public union DeleteToDoResult(NoContent, NotFound);

[Route("api/[controller]")]
[ApiController]
public class ToDoController : ControllerBase
{
    private readonly ToDoRepository repository;

    public ToDoController(ToDoRepository repository)
    {
        this.repository = repository;
    }

    // GET: api/ToDo
    /// <summary>
    /// Gets ToDo from in-memory DB
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<Ok<List<ToDo>>> GetToDo()
    {
        var items = await repository.GetToDosAsync();
        return TypedResults.Ok(items);
    }

    // GET: api/ToDo/5
    [HttpGet("{ID}", Name = "GetToDoById")]
    public async Task<Results<Ok<ToDo>, NotFound>> GetToDo(int ID)
    {
        var toDo = await repository.FindAsync(ID);

        if (toDo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(toDo);
    }

    // PUT: api/ToDo/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{ID}")]
    public async Task<Results<NoContent, BadRequest, NotFound>> PutToDo(int ID, ToDo todo)
    {
        if (ID != todo.ID)
        {
            return TypedResults.BadRequest();
        }

        try
        {
            await repository.UpdateAsync(todo);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!repository.ToDoExists(ID))
            {
                return TypedResults.NotFound(); 
            }
            else
            {
                throw;
            }
        }

        return TypedResults.NoContent();
    }

    // POST: api/ToDo
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<CreatedAtRoute<ToDo>> PostToDo(ToDo todo)
    {
        await repository.AddToDoAsync(todo);
        return TypedResults.CreatedAtRoute(todo, nameof(GetToDo), new { ID = todo.ID });
    }

    // DELETE: api/ToDo/5
    [HttpDelete("{ID}")]
    public async Task<Results<NoContent, NotFound>> DeleteToDo(int ID)
    {
        var todo = await repository.FindAsync(ID);
        if (todo == null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(todo);
        return TypedResults.NoContent();
    }
}
