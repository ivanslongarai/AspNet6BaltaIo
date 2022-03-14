using Blog.Data.Mappings;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDataContext : DbContext
    {

        public BlogDataContext(DbContextOptions<BlogDataContext> opt) : base(opt) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder options)
             => options.UseSqlServer(@"Data Source=desktop-vf2hide\sqlexpress;Initial Catalog=Blog;Integrated Security=True;Connect Timeout=30");
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new PostMap());
        }
    }
}