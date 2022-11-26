using System.ComponentModel.DataAnnotations;

namespace GameAcademy.Models
{
    public class GameRecord
    {
        [Key]
        public int id { get; set; }
        public string? gameID { get; set; }
        public string? state { get; set; } 
        public string? player1 { get; set; }
        public string? player2 { get; set; }
        public string? lastMovePlayer1 { get; set; }
        public string? lastMovePlayer2 { get; set; }

    }
}