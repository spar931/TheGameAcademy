using System.ComponentModel.DataAnnotations;

namespace GameAcademy.Models
{
    public class Product
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
        public string? Description { get; set; }
    }
}