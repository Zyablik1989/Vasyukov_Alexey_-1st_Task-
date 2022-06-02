using Microsoft.EntityFrameworkCore;
using TodoApi.DataLayer.Entities;

namespace TodoApi.DataLayer
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
           Database.EnsureCreated();
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}