using System.ComponentModel.DataAnnotations;

namespace GameAcademy.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string UserComment { get; set; } = default!;
        public string? Name { get; set; } 
        public string Ip { get; set; } = default!;
        public string Time { get; set; } = default!;
    }
}