using GameAcademy.Models;

namespace GameAcademy.Data
{
    public interface IGameAcademyRepo
    {   
        // User methods
        Task<User> addUserAsync(User user);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<bool> ValidUserAsync(string userName, string password);
        Task<User?> GetUserByUserNameAsync(string e);

        // Game methods
        Task<GameRecord> addRecordAsync(GameRecord gameRecord);
        Task<GameRecord?> GetGameRecordByGameIdAsync(string gameId);
        Task<GameRecord?> GetGameRecordByUsernameAsync(string username); 
        Task<GameRecord?> PlayerWaitingAsync();
        Task<bool> DeleteGameRecordAsync(string gameId);
        
        // Comment methods
        Task<Comment> WriteCommentAsync(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsAsync();

        // Product methods
        Task<IEnumerable<Product>> GetAllItemsAsync();

        // Save method
        Task SaveChangesAsync();
    }
}