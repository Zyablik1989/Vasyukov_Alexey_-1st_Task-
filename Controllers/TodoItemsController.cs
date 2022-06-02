using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.DataLayer;
using TodoApi.BusinessLayer;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoManipulator _todoManipulator;

        public TodoItemsController(TodoContext context)
        {
            _todoManipulator = new TodoManipulator(context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _todoManipulator.GetTodoItems();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoDto = await _todoManipulator.GetTodoDTO(id);

            if (todoDto == null)
            {
                Log.Instance.Info(this, $"Todo Item #{id} not found");
                return NotFound();
            }

            return todoDto;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                Log.Instance.Error(this, $"Failed to update Todo Item. IDs doesn't match {id}, {todoItemDTO.Id}");
                return BadRequest();
            }

            var todoDto = await _todoManipulator.UpdateTodoItem(id, todoItemDTO);
            if (todoDto == null)
            {
                Log.Instance.Error(this, $"Failed to update Todo Item. ID ({id}) not found");
                return NotFound();
            }
            else
            {
                Log.Instance.Info(this, $"Todo Item with ID ({id}) has been updated");
                return Ok(todoDto);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoDto = await _todoManipulator.CreateTodoItem(todoItemDTO);
            if (todoDto == null)
            {
                Log.Instance.Error(this, $"A problem occured with Todo Item ({todoItemDTO.Name}) creation");
                return Problem();
            }
            return todoDto;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var result = await _todoManipulator.DeleteTodoItem(id);
            if (result == false)
            {
                Log.Instance.Error(this, $"Todo Item with ID({id}) not found and Failed to be deleted");
                return NotFound();
            }
            else
            {
                Log.Instance.Info(this, $"Todo Item with ID({id}) deleted successfully");
                return Ok(result); 
            }
        }
    }
}
