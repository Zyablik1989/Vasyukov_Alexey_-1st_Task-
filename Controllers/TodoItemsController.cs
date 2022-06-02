using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return NotFound();
            }

            return todoDto;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoDto = await _todoManipulator.UpdateTodoItem(id, todoItemDTO);
            if (todoDto == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(todoDto);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoDto = await _todoManipulator.CreateTodoItem(todoItemDTO);
            if (todoDto == null)
            {
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
                return NotFound();
            }
            else
            { 
                return Ok(result); 
            }
        }
    }
}
