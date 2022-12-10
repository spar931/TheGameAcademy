using Microsoft.AspNetCore.Mvc;
using GameAcademy.Models;
using GameAcademy.Data;
using GameAcademy.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GameAcademy.Controllers
{
    [Route("api")]
    [ApiController]
    public class GameAcademyController : Controller
    {
        private readonly IGameAcademyRepo _repository;
        private readonly string _version_number;
        public GameAcademyController(IGameAcademyRepo repository)
        {
            _repository = repository;
            _version_number = "1.0.0";
        }
        
        private async Task<User?> GetLoggedUserAsync()
        {
            ClaimsIdentity? ci = HttpContext.User.Identities.FirstOrDefault();
            Claim? c = ci!.FindFirst("normalUser");
            if (c == null) 
                c= ci.FindFirst("admin");
            string userName = c!.Value;
            User? user = await _repository.GetUserByUserNameAsync(userName);
            return user;
        }


        // Miscellaneous methods 
       [HttpGet("GetLogo")]
        public ActionResult GetLogo()
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "Logo");
            string fileName1 = Path.Combine(imgDir, "logo.svg");
            string respHeader = "";
            string fileName = "";
            if (System.IO.File.Exists(fileName1))
            {
                respHeader = "image/svg+xml";
                fileName = fileName1;
            }
            else
                return NotFound();
            return PhysicalFile(fileName, respHeader);
        }

        [HttpGet("GetFavIcon")]
        public ActionResult GetFavIcon()
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "Logo");
            string fileName1 = Path.Combine(imgDir, "logo.svg");
            string respHeader = "";
            string fileName = "";
            if (System.IO.File.Exists(fileName1))
            {
                respHeader = "image/svg+xml";
                fileName = fileName1;
            }
            else
                return NotFound();
            return PhysicalFile(fileName, respHeader);
        }

        [HttpGet("ItemPhoto/{id}")]
        public ActionResult ItemPhoto(Int64 id)
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "ItemsImages");
            string fileName1 = Path.Combine(imgDir, id + ".jpg");
            string fileName2 = Path.Combine(imgDir, id + ".gif");
            string respHeader = "";
            string fileName = "";
            if (System.IO.File.Exists(fileName1))
            {
                fileName = fileName1;
                respHeader = "image/jpeg";
            }
            else if (System.IO.File.Exists(fileName2))
            {
                fileName = fileName2;
                respHeader = "image/gif";
            }
            else if (fileName == "") 
            {
                fileName = Path.Combine(imgDir, "default.png");
                respHeader = "image/png";
            }
            return PhysicalFile(fileName, respHeader); 
        }

        [HttpGet("GetVersion")]
        public ActionResult<string> GetVersion()
        {
            return _version_number;
        }   

        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpGet("GetVersionA")]
        public ActionResult<string> GetVersionA()
        {
            return _version_number + " (auth)";
        }   


        // User methods
        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterAsync(User user)
        {
            User? u = await _repository.GetUserByUserNameAsync(user.userName);
            if (u != null) {
                return "Username not available.";
            } 
            User addedUser = await _repository.addUserAsync(user);
            return $"{addedUser.userName} successfully registered.";
        }


        // Product methods
        [HttpGet("AllItems")]
        public async Task<ActionResult<IEnumerable<Product>>> AllItemsAsync()
        {
            IEnumerable<Product> Products = await _repository.GetAllItemsAsync();
            return Ok(Products);
        }

        [HttpGet("GetItems/{name}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetItemsAsync(String name)
        {
            IEnumerable<Product> Products = await _repository.GetAllItemsAsync();
            IEnumerable<Product> searched = Products.Where(e => e.Name!.ToLower().Contains(name.ToLower()));
            return Ok(searched);
        }

        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpGet("PurchaseItem/{id}")]
        public async Task<ActionResult<Order>> PurchaseItemAsync(Int64 id)
        {
            User? user = await GetLoggedUserAsync();
            Order order = new Order {userName = user!.userName, productId = id};
            return Ok(order);
        }   


        // Comment methods
        [HttpPost("WriteComment")]
        public async Task<ActionResult<string>> WriteCommentAsync(CommentIn comment)
        {
            Comment c = new Comment{UserComment = comment.userComment, Name = comment.Name};
            c.Ip = Request.HttpContext.Connection.RemoteIpAddress!.ToString();
            c.Time = DateTime.Now.ToString();
            Comment addedComment = await _repository.WriteCommentAsync(c);
            await _repository.SaveChangesAsync();
            return addedComment.UserComment;
        }   

        [HttpGet("GetComments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsAsync()
        {
            IEnumerable<Comment> Comments = await _repository.GetCommentsAsync();
            return Ok(Comments.Reverse());
        }   


        // Game Methods
        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpGet("PairMe")]
        public async Task<ActionResult<GameRecordOut>> PairMeAsync()
        {
            User? user = await GetLoggedUserAsync();
            GameRecord? playerGame = await _repository.PlayerGameStatus(user);

            GameRecordOut? gameOut;

            if (playerGame == null) {
                GameRecord? WaitingGame = await _repository.FindWaitGame();
                if (WaitingGame == null) {
                    // create a new game as player 1 if no existing game with wait status 
                    GameRecord g = new GameRecord {player1 = user!.userName, state = "wait", player2 = null, 
                        lastMovePlayer1 = null, lastMovePlayer2 = null, gameID = System.Guid.NewGuid().ToString()};

                    await _repository.addRecordAsync(g);
                    
                    gameOut = new GameRecordOut {player1 = g.player1, state = g.state, player2 = g.player2, 
                        lastMovePlayer1 = g.lastMovePlayer1, lastMovePlayer2 = g.lastMovePlayer2, gameID = g.gameID};
                } else {   
                    // join existing game with wait status as player 2
                    WaitingGame.player2 = user.userName;
                    WaitingGame.state = "progress";
                    await _repository.SaveChangesAsync();
                    gameOut = new GameRecordOut {player1 = WaitingGame.player1, state = WaitingGame.state, player2 = WaitingGame.player2, 
                        lastMovePlayer1 = WaitingGame.lastMovePlayer1, lastMovePlayer2 = WaitingGame.lastMovePlayer2, gameID = WaitingGame.gameID};
                }
            } else {
                gameOut = new GameRecordOut {player1 = playerGame.player1, state = playerGame.state, player2 = playerGame.player2, 
                    lastMovePlayer1 = playerGame.lastMovePlayer1, lastMovePlayer2 = playerGame.lastMovePlayer2, gameID = playerGame.gameID};
            }

            return Ok(gameOut);
        }

        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpGet("TheirMove/{gameId}")]
        public async Task<ActionResult<string>> TheirMoveAsync(string gameId)
        {
            User? user = await GetLoggedUserAsync();
            GameRecord? game = await _repository.GetGameRecordByGameIdAsync(gameId);

            if (game != null) {
                if (game.player1 != user!.userName && game.player2 != user.userName) {
                    return "Not your game id";
                }
                if (game.player1 == user.userName) {
                    if (game.state == "wait") {
                        return "You do not have an opponent yet";
                    }
                    if (game.lastMovePlayer2 == null) {
                        return "Your opponent has not moved yet";
                    } else {
                        return game.lastMovePlayer2;
                    }
                } 
                if (game.player2 == user.userName) {
                    if (game.state == "wait") {
                        return "You do not have an opponent yet";
                    }
                    if (game.lastMovePlayer1 == null) {
                        return "Your opponent has not moved yet";
                    }
                    else {
                        return game.lastMovePlayer1;
                    }
                } 
            }
            return "No such gameId";
        }

        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpPost("MyMove")]
        public async Task<ActionResult<string>> MyMoveAsync(GameMove gameMove)
        {
            User? user = await GetLoggedUserAsync();
            GameRecord? game = await _repository.GetGameRecordByGameIdAsync(gameMove.gameID); 

            if (game != null) {
                if (game.player1 == user!.userName) {
                    if (game.state == "wait") {
                        return "You do not have an opponent yet";
                    }
                    if (game.lastMovePlayer1 == null) {
                        game.lastMovePlayer1 = gameMove.move;
                        game.lastMovePlayer2 = null;
                        await _repository.SaveChangesAsync();
                        return "Move registered";                        
                    } else {
                        return "It is not your turn";
                    }

                } else if (game.player2 == user.userName) {
                    if (game.state == "wait") {
                        return "You do not have an opponent yet";
                    }
                    if (game.lastMovePlayer2 == null) {
                        game.lastMovePlayer2 = gameMove.move;
                        game.lastMovePlayer1 = null;
                        await _repository.SaveChangesAsync();
                        return "Move registered";
                    } else {
                        return "It is not your turn";
                    }
                } else {
                    return "Not your game id";
                }
            }
            return "No such game id";
        }

        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpGet("QuitGame/{gameId}")]
        public async Task<ActionResult<string>> QuitGameAsync(string gameId)
        {
            User? user = await GetLoggedUserAsync();
            GameRecord? game = await _repository.GetGameRecordByGameIdAsync(gameId);    

            if (game != null) {
                if (game.player1 == user!.userName || game.player2 == user.userName) {
                    await _repository.DeleteGameRecordAsync(gameId);
                    return "Game over";
                } else if (_repository.GetGameRecordByUsernameAsync(user.userName) == null) {
                    return "You have not started a game";
                } else {
                    return "Not your game id";
                }
            }
            return "No such game id";
        }
    } 
}