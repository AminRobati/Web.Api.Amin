using Microsoft.EntityFrameworkCore;
using Web.Api.Amin.Models.Entities;

namespace Web.Api.Amin.Models.Contexts
{
    public class DataBaseContext:DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<SmsCode> SmsCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDo>().HasQueryFilter(p=>!p.IsRemoved);
        }

    }
}
