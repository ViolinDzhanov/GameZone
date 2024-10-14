using System.ComponentModel.DataAnnotations;

namespace GameZone.Models
{
    public class GameDetailsViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string ReleasedOn { get; set; } = null!;

        [Required]
        public string Genre { get; set; } = null!;

        [Required]
        public string Publisher { get; set; } = null!;
    }
}
