using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book
    {
        public string? Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Author { get; set; }

        [Required]
        public string? Publisher { get; set; }

        [Required]
        [DisplayName("Publishing date")]
        [DataType(DataType.Date)]
        public DateTime? PublishingDate { get; set; }

        [Required]
        public string? Genre { get; set; }

        [Required]
        [DisplayName("ISBN")]
        public string? Isbn { get; set; }

        [DisplayName("Available")]
        public bool IsAvailable { get; set; }

        [DisplayName("Unavailable until")]
        public DateTime? UnavailableUntil { get; set; }

        [DisplayName("Taken by")]
        public string? TakenBy { get; set; }
       
    }
}
