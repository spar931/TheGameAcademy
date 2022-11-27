using System.ComponentModel.DataAnnotations;

namespace GameAcademy.Dtos
{
    public class CommentIn
    {
        [Required]
        public string userComment { get; set; } = default!;
        public string? Name { get; set; }
    }
}