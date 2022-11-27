using Microsoft.EntityFrameworkCore;
using GameAcademy.Models;

namespace GameAcademy.Data
{
    public class GameAcademyDBContext : DbContext 
    {
        public GameAcademyDBContext(DbContextOptions<GameAcademyDBContext> options) : base(options) {}
        public DbSet<User> Users{get; set;}
        public DbSet<GameRecord> GameRecords{get; set;}
        public DbSet<Product> Products{get; set;}
        public DbSet<Comment> Comments{get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=GameAcademyDatabase.sqlite");
        }
    }
}