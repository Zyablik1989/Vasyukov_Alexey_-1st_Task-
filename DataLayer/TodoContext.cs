using Microsoft.EntityFrameworkCore;
using System;
using TodoApi.DataLayer.Entities;

namespace TodoApi.DataLayer
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
            try
            {
                if (Database.EnsureCreated())
                {
                    Log.Instance.Error(this, $"Database 'ToDoDb' not found. A new one has been created.");
                }
            }
            catch(Exception e)
            {
                Log.Instance.Error(this, e);
            }
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}