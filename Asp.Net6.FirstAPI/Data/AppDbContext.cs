using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoModel> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlite("DataSource=app.db;Cache=Shared");
        }
    }
}
