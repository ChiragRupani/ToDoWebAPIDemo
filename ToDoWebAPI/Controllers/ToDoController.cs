using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoWebAPI.Models;
using ToDoWebAPI.Repository;

namespace ToDoWebAPI.Controllers
{
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetToDo(int id)
        {
            var toDo = await repository.FindAsync(id);

            if (toDo == null)
            {
                return NotFound();
            }

            return toDo;
        }

        // PUT: api/ToDo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDo(int id, ToDo todo)
        {
            if (id != todo.ID)
            {
                return BadRequest();
            }

            try
            {
                await repository.UpdateAsync(todo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!repository.ToDoExists(id))
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
            return CreatedAtAction(nameof(GetToDo), new { id = todo.ID }, todo);
        }

        // DELETE: api/ToDo/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            var todo = await repository.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(todo);
            return NoContent();
        }
    }
}
