using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.DataLayer;
using TodoApi.DataLayer.Entities;
using TodoApi.Models;

namespace TodoApi.BusinessLayer
{
    public class TodoManipulator
    {
        private readonly TodoContext _context;


        public TodoManipulator(TodoContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<TodoItemDTO>>>  GetTodoItems()
        {
            return await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        public async Task<TodoItem> GetTodoItem(long id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<TodoItemDTO> GetTodoDTO(long id)
        {
            var todoItem = await GetTodoItem(id);
            if (todoItem == null)
            { 
                return null; 
            }

            return ItemToDTO(todoItem);

        }

        public async Task<TodoItemDTO> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            var todoItem = await GetTodoItem(id);
            if (todoItem == null)
            {
                return null;
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return null;
            }

            return ItemToDTO(GetTodoItem(id).Result);
        }

        public async Task<TodoItemDTO> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            var todoDto = ItemToDTO(todoItem);
            return todoDto;
        }

        public async Task<bool> DeleteTodoItem(long id)
        {
            var todoItem = await GetTodoItem(id);

            if (todoItem == null)
            {
                return false;
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return true;
        }


        private static TodoItemDTO TodoEntityToDTO(TodoItem todoEntity)
        {
            if (todoEntity == null) return null;

            return new TodoItemDTO
            {
                Id = todoEntity.Id,
                Name = todoEntity.Name,
                IsComplete = todoEntity.IsComplete
            };
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem)
        {
            if (todoItem == null) return null;
                 
            return new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);



    }
}
