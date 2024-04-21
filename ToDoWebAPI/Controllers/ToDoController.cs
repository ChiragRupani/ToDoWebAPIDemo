using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoWebAPI.Models;
using ToDoWebAPI.Repository;

namespace ToDoWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
public class ToDoController : ControllerBase
{
    private readonly ToDoRepository repository;

    public ToDoController(ToDoRepository repository)
    {
        this.repository = repository;
    }

    // GET: api/ToDo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDo>>> GetToDo()
    {
        return await repository.GetToDosAsync();
    }

    // GET: api/ToDo/5
    [HttpGet("{ID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ToDo>> GetToDo(int ID)
    {
        var toDo = await repository.FindAsync(ID);

        if (toDo is null)
        {
            return NotFound();
        }

        return toDo;
    }

    // PUT: api/ToDo/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{ID}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutToDo(int ID, ToDo todo)
    {
        if (ID != todo.ID)
        {
            return BadRequest();
        }

        try
        {
            await repository.UpdateAsync(todo);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!repository.ToDoExists(ID))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/ToDo
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ToDo>> PostToDo(ToDo todo)
    {
        await repository.AddToDoAsync(todo);
        return CreatedAtAction(nameof(GetToDo), new { ID = todo.ID }, todo);
    }

    // DELETE: api/ToDo/5
    [HttpDelete("{ID}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteToDo(int ID)
    {
        var todo = await repository.FindAsync(ID);
        if (todo == null)
        {
            return NotFound();
        }

        await repository.DeleteAsync(todo);
        return NoContent();
    }
}
