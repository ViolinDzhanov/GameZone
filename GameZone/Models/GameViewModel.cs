using GameZone.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static GameZone.Constants.ModelConstants.Game;

namespace GameZone.Models
{
    public class GameViewModel
    {
        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = "Titleshould be between 2 and 50 symbols")]
        public string Title { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = "Description should be between 10 and 500 symbols")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ReleasedOn { get; set; } = DateTime.Now.ToString(ReleasedOnFormat);

        [Required]
        public int GenreId { get; set; }

        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
