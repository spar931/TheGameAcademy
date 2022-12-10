using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using GameAcademy.Models;

namespace GameAcademy.Data
{
    public class GameAcademyRepo : IGameAcademyRepo
    {
        private readonly GameAcademyDBContext _dbContext;

        public GameAcademyRepo(GameAcademyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // User methods
        public async Task<User> addUserAsync(User user) 
        {
            ValueTask<EntityEntry<User>> e = _dbContext.Users.AddAsync(user);
            await e;
            User u = e.Result.Entity;
            await SaveChangesAsync();
            return u;
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            IEnumerable<User> users = await _dbContext.Users.ToListAsync<User>(); 
            return users;
        }
        public async Task<bool> ValidUserAsync(string userName, string password)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(e => e.userName == userName && e.password == password);
            if (user == null)
                return false;
            else
                return true;
        }
        public async Task<User?> GetUserByUserNameAsync(string username)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(e => e.userName == username);
            return user;
        }
        

        // Game methods
        public async Task<GameRecord> addRecordAsync(GameRecord gameRecord)
        {
            ValueTask<EntityEntry<GameRecord>> e = _dbContext.GameRecords.AddAsync(gameRecord);
            await e;
            GameRecord u = e.Result.Entity;
            await SaveChangesAsync();
            return u;
        }
        public async Task<GameRecord?> GetGameRecordByGameIdAsync(string gameId) 
        {
            GameRecord? gameRecord = await _dbContext.GameRecords.FirstOrDefaultAsync(e => e.gameID == gameId);
            return gameRecord;
        }
        public async Task<GameRecord?> GetGameRecordByUsernameAsync(string username) 
        {
            GameRecord? gameRecord = await _dbContext.GameRecords.FirstOrDefaultAsync(e => e.player1 == username || e.player2 == username);
            return gameRecord;
        }
        public async Task<GameRecord?> PlayerWaitingAsync()
        {
            GameRecord? gameRecord = await _dbContext.GameRecords.FirstOrDefaultAsync(e => e.state == "wait");
            return gameRecord;
        }
        public async Task<GameRecord?> PlayerInGame(User user)
        {
            GameRecord? gameRecord = await _dbContext.GameRecords.FirstOrDefaultAsync(e => e.state == "progress" && (e.player1 != user.userName || 
            e.player2 != user.userName));
            return gameRecord;
        }
        public async Task<bool> DeleteGameRecordAsync(string gameId) 
        {
            GameRecord? gameRecord = await _dbContext.GameRecords.FirstOrDefaultAsync(e => e.gameID == gameId);
            if (gameRecord == null)
                return false;
            _dbContext.GameRecords.Remove(gameRecord);
            await SaveChangesAsync();
            return true;
        }

        // Comment methods
        public async Task<Comment> WriteCommentAsync(Comment comment)
        {
            ValueTask<EntityEntry<Comment>> e =  _dbContext.Comments.AddAsync(comment);
            await e;
            Comment c = e.Result.Entity;
            await SaveChangesAsync();
            return c;
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            IEnumerable<Comment> comments = await _dbContext.Comments.ToListAsync<Comment>(); 
            return comments;
        }
        
        // Product methods
        public async Task<IEnumerable<Product>> GetAllItemsAsync()
        {
            IEnumerable<Product> products = await _dbContext.Products.ToListAsync<Product>(); 
            return products;
        }
        public async Task<Product?> GetProductById(string productId)
        {
            Product? product = await _dbContext.Products.FirstOrDefaultAsync(e => e.Id == productId);
            return product;
        }

        // Save method
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}