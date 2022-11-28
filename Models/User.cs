using System.ComponentModel.DataAnnotations;

namespace GameAcademy.Models
{
    public class User
    {
        [Key]
        [Required]
        public string userName { get; set; } = default!;
        public string password { get; set; } = default!;
        public string email { get; set; } = default!;
    }
}