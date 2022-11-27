using Microsoft.EntityFrameworkCore;
using GameAcademy.Models;

namespace GameAcademy.Data
{
    public class GameAcademyDBContext : DbContext 
    {
        public GameAcademyDBContext(DbContextOptions<GameAcademyDBContext> options) : base(options) {}
        public DbSet<User> Users{get; set;} = null!;
        public DbSet<GameRecord> GameRecords{get; set;} = null!;
        public DbSet<Product> Products{get; set;} = null!;
        public DbSet<Comment> Comments{get; set;} = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=GameAcademyDatabase.sqlite");
        }
    }
}