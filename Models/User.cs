using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class User
    {
        public string? Id { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }
    }
}
